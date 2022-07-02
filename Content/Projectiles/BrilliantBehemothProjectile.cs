using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Items.Weapons;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Projectiles
{
    internal class BrilliantBehemothProjectile : ModProjectile
    {
        enum Phase { Normal, Exploding }

        private int Charge
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private Phase AIPhase
        {
            get => (Phase)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        private float Radius => BrilliantBehemoth.BaseBlastRadius + Charge * BrilliantBehemoth.BlastRadiusIncreaseMultiplier;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Behemoth Rocket");
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
            if (AIPhase == Phase.Exploding)
            {
                return ProjectileUtils.IsRectangleWithinRadius(Projectile.Center, Radius, targetHitbox);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Explode();
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Explode on last regular hit
            if (AIPhase != Phase.Exploding && Projectile.penetrate == 1)
            {
                Explode();
            }
        }

        public override void AI()
        {
            if (AIPhase == Phase.Exploding)
            {
                return;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            // Dust
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.TopLeft, Projectile.width / 2, Projectile.height, DustID.Smoke);
            }
            if (Main.rand.NextBool(4) && Charge > BrilliantBehemoth.ClusterThresholdCharge / 2)
            {
                Dust.NewDust(Projectile.TopLeft, Projectile.width / 2, Projectile.height, DustID.SilverFlame);
            }
            if (Main.rand.NextBool(4) && Charge > BrilliantBehemoth.ClusterThresholdCharge)
            {
                Dust.NewDust(Projectile.TopLeft, Projectile.width / 2, Projectile.height, DustID.GoldFlame);
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

            DustUtils.Explosion(Projectile.Center, Radius);
            SoundEngine.PlaySound(RoRSound.BehemothBlast.WithVolumeScale(0.3f), Projectile.Center);
        }

        public override void Kill(int timeLeft)
        {
            // Create cluster blasts if charged enough
            if (Charge >= BrilliantBehemoth.ClusterThresholdCharge && PlayerUtils.IsOwnerClient(Projectile.owner))
            {
                CreateClusterBlasts();
            }
        }

        private void CreateClusterBlasts()
        {
            int explosionType = ModContent.ProjectileType<BehemothExplosionProjectile>();
            for (float t = 0; t < MathHelper.TwoPi; t += MathHelper.PiOver4)
            {
                Vector2 position = Projectile.Center + t.ToRotationVector2() * Radius;
                ProjectileUtils.CreateExplosion(Main.player[Projectile.owner], Projectile.GetSource_FromThis(),
                    position, Radius / 2, Projectile.damage / 2, Projectile.knockBack, BrilliantBehemoth.ClusterDelay, 
                    projectileType: explosionType);
            }
        }
    }
}
