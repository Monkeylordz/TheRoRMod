using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using RoRMod.Content.Items.Accessories;
using RoRMod.Utilities;
using ReLogic.Utilities;
using Terraria.Audio;
using System;
using RoRMod.Content.Common;

namespace RoRMod.Content.Projectiles
{
    internal class ChargefieldProjectile : ModProjectile
    {
        public const int VertexCount = 48;

        private const int SoundDelay = 150;
        private SlotId loopSoundSlotID = SlotId.Invalid;

        private int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public int Charge
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        private float Radius => ChargefieldGenerator.InitialRadius + ChargefieldGenerator.RadiusIncrease * Charge;
        private Vector2[] vertexOffsets = new Vector2[VertexCount];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chargefield");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Collides if target hitbox is within range of the field
            return ProjectileUtils.IsRectangleWithinRadius(Projectile.Center, Radius, targetHitbox);
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(RoRSound.ChargefieldStart.WithVolumeScale(0.15f), Projectile.Center);
        }

        public override void AI()
        {
            // Kill if player died/left
            if (!Main.player[Projectile.owner].active)
            {
                Projectile.Kill();
                return;
            }

            // Center on player
            Projectile.Center = Main.player[Projectile.owner].Center;

            // Sound
            if (Projectile.soundDelay <= 0 || loopSoundSlotID == SlotId.Invalid)
            {
                Projectile.soundDelay = SoundDelay;
                loopSoundSlotID = SoundEngine.PlaySound(RoRSound.ChargefieldLoop.WithVolumeScale(0.25f), Projectile.Center);
            }

            // Stay alive forever
            Projectile.timeLeft = 2;

            Timer++;
        }

        private void RandomizeVertexOffsets()
        {
            // Randomize directions
            float[] rotations = new float[VertexCount];
            for (int i = 0; i < VertexCount; i++)
            {
                rotations[i] = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            Array.Sort(rotations);

            // Convert rotations to Vector2 at a randomized radius
            for (int i = 0; i < VertexCount; i++)
            {
                float perturbedRadius = Radius * Main.rand.NextFloat(0.95f, 1.05f);
                vertexOffsets[i] = rotations[i].ToRotationVector2() * perturbedRadius;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Randomize offsets every 5 frames
            if (Timer % 5 == 0)
            {
                RandomizeVertexOffsets();
            }

            // offset vertices by position and draw
            Vector2[] vertices = new Vector2[VertexCount + 1];
            for (int i = 0; i < VertexCount; i++)
            {
                vertices[i] = Projectile.Center + vertexOffsets[i];
            }
            vertices[VertexCount] = vertices[0];
            DrawUtils.DrawLineStrip(vertices, Color.White);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            // Sounds
            SoundEngine.PlaySound(RoRSound.ChargefieldStop.WithVolumeScale(0.15f), Projectile.Center);
            if (loopSoundSlotID != SlotId.Invalid && SoundEngine.TryGetActiveSound(loopSoundSlotID, out ActiveSound sound))
            {
                sound.Stop();
            }
        }
    }
}
