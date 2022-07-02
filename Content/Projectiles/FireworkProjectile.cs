using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using RoRMod.Content.Items.Weapons;
using RoRMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class FireworkProjectile : ModProjectile
    {
        private enum Phase { Default, Exploding }

        private Phase AIPhase
        {
            get => (Phase)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Firework");
        }

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 16;
			Projectile.aiStyle = 0;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = false;
			Projectile.light = 0.5f;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 600;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Exploding: collision within radius
            if (AIPhase == Phase.Exploding)
            {
                return ProjectileUtils.IsRectangleWithinRadius(Projectile.Center, BundleOfFireworks.BlastRadius, targetHitbox);
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

        public override void OnSpawn(IEntitySource source)
        {
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

        public override void AI()
        {
            if (AIPhase == Phase.Exploding)
            {
                return;
            }

            // Dust
            if (Main.rand.NextBool())
            {
				Dust.NewDust(Projectile.Left, 0, Projectile.height, DustID.Flare);
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

            // Randomly choose dust from 5 firework dusts
            int dustID = DustID.Firework_Red + Main.rand.Next(5);
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f) * 8f;
                Dust.NewDustPerfect(Projectile.position, dustID, Velocity: velocity);
            }

            // Firework Explosion Sound
            SoundEngine.PlaySound(RoRSound.FireworkExplosion.WithVolumeScale(0.5f), Projectile.Center);
        }
    }
}
