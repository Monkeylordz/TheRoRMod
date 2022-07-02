using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class HeavenCrackerFissureBaseProjectile : ModProjectile
    {
        public override string Texture => "RoRMod/Content/Items/Weapons/HeavenCracker";

        // Duration: Lifespan of the projectiles
        public const int Duration = 120;

        // FadeDelay: Frames to wait before projectile starts to fade
        public const int FadeDelay = 60;

        // ForwardOffsetAmount: Distance in front of projectile that fissures are spawned
        public const float ForwardOffsetAmount = 50f;

        // BackwardOffsetAmount: Distance behind projectile for back vertex
        public const float BackwardOffsetAmount = 5f;

        // FissureCount: The number of fissures to spawn
        public const int MinFissureCount = 3;
        public const int MaxFissureCount = 5;
        public int FissureCount;

        // FissureSeparationAngle: The angle between two adjacent fissures 
        public const float SeparationAngleBase = MathHelper.Pi / 12;
        public const float SeparationAngleRange = MathHelper.Pi / 24;
        public float SeparationAngle;

        // CrackHalfWidth: Half of the width of a fissure segment
        public const float CrackHalfWidth = 15f;

        // CrackLength: The length of a fissure segment
        public const float MinCrackLength = 40f;
        public const float MaxCrackLength = 80f;
        public float CrackLength;

        // CrackCount: The number of cracks in fissure
        public const int MinCrackCount = 3;
        public const int MaxCrackCount = 8;

        private int Timer
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        private Vector2 ForwardOffset => Projectile.velocity.SafeNormalize(-Vector2.UnitY) * ForwardOffsetAmount;
        private Vector2 BackwardOffset => -Projectile.velocity.SafeNormalize(-Vector2.UnitY) * BackwardOffsetAmount;
        private Vector2 Normal => Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(-Vector2.UnitY);
        private int VertexCount => FissureCount + 2;
        private float Alpha => (Timer >= FadeDelay) ? (float)(Duration - Timer) / (Duration - FadeDelay) : 1;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(FissureCount);
            writer.Write(SeparationAngle);
            writer.Write(CrackLength);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            FissureCount = reader.ReadInt32();
            SeparationAngle = reader.ReadSingle();
            CrackLength = reader.ReadSingle();
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dimensional Fissure");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Duration;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            // TODO: Check hitboxes
            hitbox.Inflate((int)ForwardOffsetAmount, (int)ForwardOffsetAmount);
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (PlayerUtils.IsOwnerClient(Projectile.owner))
            {
                // Initialize random variables
                FissureCount = Main.rand.Next(MinFissureCount, MaxFissureCount + 1);
                SeparationAngle = SeparationAngleBase + SeparationAngleRange * Main.rand.NextFloat(-1, 1);
                CrackLength = Main.rand.NextFloat(MinCrackLength, MaxCrackLength);

                // Spawn Subprojectiles
                SpawnFissures();
            }
            SoundEngine.PlaySound(RoRSound.Fissure.WithVolumeScale(0.33f), Projectile.position);

            // Set projectile variables
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.oldPosition = Projectile.position;
        }

        public override void AI()
        {
            // Stay still
            Projectile.position = Projectile.oldPosition;

            Timer++;
        }

        private void SpawnFissures()
        {
            // c is the distance between two consecutive positions
            float c = MathF.Sqrt(2 * CrackHalfWidth * CrackHalfWidth * (1 - MathF.Cos(MathHelper.Pi - SeparationAngle)));

            // Loop for both left and right sides
            foreach (int sign in new int[] { 1, -1 })
            {
                Vector2 normal = sign * Normal;

                // Different starting positions for odd/even fissure counts
                Vector2 position = Projectile.position + ForwardOffset;
                if (FissureCount % 2 == 0)
                {
                    // Even: start with angle offset
                    normal = normal.RotatedBy(sign * SeparationAngle);
                    position += normal * CrackHalfWidth;
                    SpawnFissure(position, normal, sign);
                }
                else
                {
                    // Odd: Spawn fissure at default position, only perform once
                    if (sign == 1)
                    {
                        SpawnFissure(position, normal, sign);
                    }
                }
                
                for (int i = 0; i < (FissureCount - 1) / 2; i++)
                {
                    // Calculate next position and spawn a fissure there
                    position = position + normal.RotatedBy(sign * SeparationAngle / 2) * c;
                    normal = normal.RotatedBy(sign * SeparationAngle);
                    SpawnFissure(position, normal, sign);
                }
            }
        }

        private void SpawnFissure(Vector2 position, Vector2 normal, int sign)
        {
            IEntitySource spawnSource = Projectile.GetSource_FromThis();
            int crackCount = Main.rand.Next(MinCrackCount, MaxCrackCount);
            int projectileType = ModContent.ProjectileType<HeavenCrackerFissureProjectile>();
            Vector2 velocity = normal.RotatedBy(-sign * MathHelper.PiOver2) * CrackLength;
            position -= velocity;
            int projID = Projectile.NewProjectile(spawnSource, position, velocity,
                projectileType, Projectile.damage / 2, Projectile.knockBack, Projectile.owner, ai1: crackCount);
            Main.projectile[projID].position = position;
            Main.projectile[projID].oldPosition = position;
        }

        private Vector2[] GetVerticesWorld()
        {
            // FissureCount + 1 vertices between fissure centers + 1 vertex inside drill
            Vector2[] vertices = new Vector2[VertexCount];
            int midpointIndex = FissureCount / 2 + 1;

            Vector2 normalLeft = Normal;
            Vector2 normalRight = -Normal;

            // Vertices in front
            vertices[0] = Projectile.position + ForwardOffset;
            if (FissureCount % 2 == 1)
            {
                // Odd number of fissures: offset forward vertices
                Vector2 center = vertices[0];
                vertices[0] = center + normalLeft * CrackHalfWidth;
                vertices[vertices.Length - 1] = center + normalRight * CrackHalfWidth;
            }
            
            // Vertices from fissure corners
            for (int i = 1; i < midpointIndex; i++)
            {
                // Left: Add normally
                normalLeft = normalLeft.RotatedBy(SeparationAngle);
                vertices[i] = vertices[i - 1] + normalLeft * 2 * CrackHalfWidth;

                // Right: Add in reverse
                normalRight = normalRight.RotatedBy(-SeparationAngle);
                int j = FissureCount % 2 == 1 ? i + 1 : i;
                vertices[vertices.Length - j] = vertices[(vertices.Length - j + 1) % vertices.Length] + normalRight * 2 * CrackHalfWidth;
            }

            // Vertex inside drill
            vertices[midpointIndex] = Projectile.position + BackwardOffset;

            return vertices;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Create vertex data for polygon
            Vector2[] verticesWorld = GetVerticesWorld();
            int vertexBufferLength = 3 * FissureCount;
            VertexPositionColor[] vertices = new VertexPositionColor[vertexBufferLength];
            for (int i = 0; i < FissureCount; i++)
            {
                vertices[i * 3] = DrawUtils.WorldToVertexPositionColor(verticesWorld[0], Color.White);
                vertices[i * 3 + 1] = DrawUtils.WorldToVertexPositionColor(verticesWorld[i + 1], Color.White);
                vertices[i * 3 + 2] = DrawUtils.WorldToVertexPositionColor(verticesWorld[i + 2], Color.White);
            }

            VertexBuffer vertexBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionColor), vertexBufferLength, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            // Draw polygon
            DrawFissurePolygon(vertexBuffer, PrimitiveType.TriangleList, FissureCount, Alpha);

            return false;
        }

        public static void DrawFissurePolygon(VertexBuffer vertexBuffer, PrimitiveType primitiveType, int primitiveCount, float alpha)
        {
            // Set shader parameters
            Effect shader = RoRShaders.GetShader(RoRShaders.ShaderID.FissureEffect2x2);
            shader.Parameters["uThreshold"].SetValue(0.9f); // Threshold for star brightness
            shader.Parameters["uScale"].SetValue(32f); // Scale of Worley noise
            shader.Parameters["uDefaultColor"].SetValue(new Vector3(0.25f, 0, 0.375f)); // Dark purple default background (0x400060)
            shader.Parameters["uAlpha"].SetValue(alpha); // Alpha

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            BlendState prevBlendState = device.BlendState;
            device.BlendState = BlendState.NonPremultiplied;
            device.SetVertexBuffer(vertexBuffer);
            foreach (EffectPass effectPass in shader.CurrentTechnique.Passes)
            {
                // Apply Shader pass
                effectPass.Apply();

                // Draw primitives to form polygon
                device.DrawPrimitives(primitiveType, 0, primitiveCount);
            }
            device.BlendState = prevBlendState;
        }
    }
}
