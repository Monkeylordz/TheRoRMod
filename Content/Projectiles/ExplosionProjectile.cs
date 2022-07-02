using Microsoft.Xna.Framework;
using RoRMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class ExplosionProjectile : ModProjectile
    {
        protected float Radius
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        protected int Delay
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1.5f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Delay > 0)
            {
                return false;
            }
            return ProjectileUtils.IsRectangleWithinRadius(Projectile.Center, Radius, targetHitbox);
        }

        public override void AI()
        {
            if (Delay > 0)
            {
                // Wait for delay
                Delay--;
                Projectile.timeLeft = 5;
                return;
            }
            else if (Delay == 0)
            {
                // One-time explosion
                Delay--;
                Explode();
            }
        }

        protected virtual void Explode()
        {
            DustUtils.Explosion(Projectile.Center, Radius);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
