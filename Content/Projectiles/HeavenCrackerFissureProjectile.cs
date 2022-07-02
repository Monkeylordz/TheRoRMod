using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RoRMod.Utilities;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class HeavenCrackerFissureProjectile : ModProjectile
    {
        public override string Texture => "RoRMod/Content/Items/Weapons/HeavenCracker";

        public const int SpawnDelay = 5;

        public const float CrackAngleBase = MathHelper.Pi / 16;
        public const float CrackAngleRange = MathHelper.Pi / 32;
        public float CrackAngle;

        private int Duration => HeavenCrackerFissureBaseProjectile.Duration;
        private int FadeDelay => HeavenCrackerFissureBaseProjectile.FadeDelay;
        private float Alpha => (Timer >= FadeDelay) ? (float)(Duration - Timer) / (Duration - FadeDelay) : 1;

        private int Timer
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        private int CrackIndex
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }

        private Vector2 Normal => Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(-Vector2.UnitY);
        private bool IsTip => CrackIndex == 0 || Timer < SpawnDelay;

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(CrackAngle);
        public override void ReceiveExtraAI(BinaryReader reader) => CrackAngle = reader.ReadSingle();

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

        public override void OnSpawn(IEntitySource source)
        {
            if (PlayerUtils.IsOwnerClient(Projectile.owner))
            {
                CrackAngle = CrackAngleBase + CrackAngleRange * Main.rand.NextFloat(-1, 1);
            }

            Projectile.oldPosition = Projectile.position;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            // Don't move
            Projectile.position = Projectile.oldPosition;

            // Spawn Fissure if FissureIndex > 0
            if (Timer == SpawnDelay && CrackIndex > 0 && PlayerUtils.IsOwnerClient(Projectile.owner))
            {
                SpawnNextFissure();
            }

            Timer++;
        }

        private void SpawnNextFissure()
        {
            IEntitySource spawnSource = Projectile.GetSource_FromThis();
            Vector2 position = GetNextPosition();
            int projID = Projectile.NewProjectile(spawnSource, position, Projectile.velocity,
                Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: CrackIndex - 1);
            Main.projectile[projID].position = position;
            Main.projectile[projID].oldPosition = position;
        }

        private Vector2 GetNextPosition()
        {
            // CrackIndex determines the direction of the crack. Add a little bit of randomness.
            float rotation = ((CrackIndex % 2 == 0) ? 1 : -1) * CrackAngle;
            return Projectile.position + Projectile.velocity.RotatedBy(rotation);
        }

        private Vector2[] GetVerticesWorld()
        {
            if (IsTip)
            {
                Vector2[] vertices = new Vector2[3]
                {
                    Projectile.position + Normal * HeavenCrackerFissureBaseProjectile.CrackHalfWidth,
                    Projectile.position - Normal * HeavenCrackerFissureBaseProjectile.CrackHalfWidth,
                    GetNextPosition()
                };
                return vertices;
            }
            else
            {
                Vector2[] vertices = new Vector2[4]
                {
                    Projectile.position + Normal * HeavenCrackerFissureBaseProjectile.CrackHalfWidth,
                    Projectile.position - Normal * HeavenCrackerFissureBaseProjectile.CrackHalfWidth,
                    GetNextPosition() + Normal * HeavenCrackerFissureBaseProjectile.CrackHalfWidth,
                    GetNextPosition() - Normal * HeavenCrackerFissureBaseProjectile.CrackHalfWidth
                };
                return vertices;
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            // TODO: check hitboxes
            hitbox.Width = (int)Projectile.velocity.Length();
            hitbox.Height = (int)HeavenCrackerFissureBaseProjectile.CrackHalfWidth * 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Create vertex data for polygon
            VertexPositionColor[] vertices = DrawUtils.WorldToVertexPositionColor(GetVerticesWorld());
            VertexBuffer vertexBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
            HeavenCrackerFissureBaseProjectile.DrawFissurePolygon(vertexBuffer, PrimitiveType.TriangleStrip, vertices.Length - 2, Alpha);

            return false;
        }
    }
}
