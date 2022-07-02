using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Items.Accessories;
using RoRMod.Utilities;
using Terraria.DataStructures;
using System;
using Terraria.Audio;
using System.IO;
using RoRMod.Content.Common;

namespace RoRMod.Content.Projectiles
{
    internal class StickyBombProjectile : ModProjectile
    {
        public const int Duration = 180;

        private enum Phase { StickToNPC, Fall, Explode }

        private Phase AIPhase
        {
            get { return (Phase)Projectile.ai[0]; }
            set { Projectile.ai[0] = (float)value; }
        }

        private int AttachedNPC
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }

        private int BeepDelay
        {
            get { return (int)Projectile.localAI[1]; }
            set { Projectile.localAI[1] = value; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sticky Bomb");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 22;
            Projectile.scale = 0.75f;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Duration;
            Projectile.penetrate = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Only collide when exploding
            if (AIPhase != Phase.Explode) return false;

            // Use sticky bomb blast radius
            return ProjectileUtils.IsRectangleWithinRadius(Projectile.Center, StickyBomb.BlastRadius, targetHitbox);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            BeepDelay = 30;
            Projectile.soundDelay = BeepDelay;
            Projectile.rotation = Projectile.velocity.RotatedByRandom(0.2f).ToRotation();

            SoundEngine.PlaySound(RoRSound.StickyBombActivate.WithVolumeScale(0.5f), Projectile.Center);
        }

        public override void AI()
        {
            switch (AIPhase)
            {
                case Phase.StickToNPC:
                    StickToNPC();
                    break;
                case Phase.Fall:
                    GravityAndDrag();
                    break;
                case Phase.Explode:
                    return;
            }

            Countdown();

            // Explode on time up
            if (Projectile.timeLeft < 5)
            {
                Explode();
            }
        }

        private void StickToNPC()
        {
            NPC npc = Main.npc[AttachedNPC];

            // Check if we are still attached to an NPC
            if (!npc.active)
            {
                AIPhase = Phase.Fall;
                Projectile.tileCollide = true;
                return;
            }

            Projectile.Center = npc.Center - Projectile.velocity;
            Projectile.gfxOffY = npc.gfxOffY;
        }

        private void GravityAndDrag()
        {
            Projectile.velocity.Y = MathF.Min(Projectile.velocity.Y + 0.2f, 40);
            Projectile.velocity.X *= 0.95f;
        }

        // Blink and beep, gets progressively faster
        private void Countdown()
        {
            if (Projectile.soundDelay == 0)
            {
                BlinkAndBeep();
                if (BeepDelay > 4)
                {
                    BeepDelay = (int)(BeepDelay * 0.8f);
                }
                Projectile.soundDelay = BeepDelay;
            }
        }

        private void BlinkAndBeep()
        {
            // Flip between frames and make countdown sound
            if (Projectile.frame == 0)
            {
                Projectile.frame = 1;
                float pitch = 0.5f * (Duration - Projectile.timeLeft) / Duration;
                SoundEngine.PlaySound(RoRSound.StickyBombCountdown.WithVolumeScale(0.5f).WithPitchOffset(pitch), Projectile.Center);
            }
            else
            {
                Projectile.frame = 0;
            }
        }

        private void Explode()
        {
            AIPhase = Phase.Explode;
            Projectile.timeLeft = 4;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            Projectile.velocity = Vector2.Zero;

            DustUtils.Explosion(Projectile.Center, StickyBomb.BlastRadius);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
    }
}
