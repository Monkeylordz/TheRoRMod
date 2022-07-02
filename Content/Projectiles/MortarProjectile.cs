using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Items.Accessories;
using System;
using System.Collections.Generic;
using Terraria.Audio;

namespace RoRMod.Content.Projectiles
{
    internal class MortarProjectile : ModProjectile
    {
        private enum Phase { Default, Exploding }

        private Phase AIPhase
        {
            get => (Phase)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mortar Shell");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (AIPhase == Phase.Exploding)
            {
                return ProjectileUtils.IsRectangleWithinRadius(Projectile.Center, MortarTube.ShellBlastRadius, targetHitbox);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Explode on last regular hit
            if (AIPhase != Phase.Exploding && Projectile.penetrate == 1)
            {
                Explode();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Explode();
            return false;
        }

        public override void AI()
        {
            if (AIPhase == Phase.Exploding)
            {
                return;
            }

            // Gravity
            Projectile.velocity.Y = Math.Min(Projectile.velocity.Y + MortarTube.Gravity, 100);
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Dust
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.Left, 0, Projectile.height, DustID.OrangeTorch);
            }

            // Explode on time up
            if (Projectile.timeLeft < 5)
            {
                Explode();
            }
        }

        private void Explode()
        {
            AIPhase = Phase.Exploding;
            Projectile.timeLeft = 4;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            Projectile.velocity = Vector2.Zero;
            Projectile.penetrate = -1;

            DustUtils.Explosion(Projectile.Center, MortarTube.ShellBlastRadius);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
    }
}
