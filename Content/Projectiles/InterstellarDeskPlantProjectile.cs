using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class InterstellarDeskPlantProjectile : ModProjectile
    {
        public const int Lifetime = 300;
        public const float BaseHealRadius = 250;
        public const int HealInterval = 20;

        private float HealRadius
        {
            get { return Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Interstellar Desk Plant");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Lifetime;
            Projectile.light = 2f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(RoRSound.PlantGrow.WithVolumeScale(0.75f), Projectile.Center);
        }

        public override void AI()
        {
            // Dust Ring
            if (Projectile.timeLeft % 5 == 0)
            {
                HealRadius = BaseHealRadius * Projectile.timeLeft / Lifetime;

                for (int i = 0; i < 30; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(HealRadius, HealRadius);
                    Dust d = Dust.NewDustPerfect(Projectile.Center + offset, DustID.ChlorophyteWeapon);
                    d.noGravity = true;
                }
            }

            // Heal Players inside radius for 1% life
            if (Projectile.timeLeft % HealInterval == 0)
            {
                float HealRadiusSqr = HealRadius * HealRadius;
                Player player = Main.player[Projectile.owner];
                if (player != null)
                {
                    if (Vector2.DistanceSquared(player.Center, Projectile.Center) < HealRadiusSqr)
                    {
                        PlayerUtils.HealPercentage(player, 0.01f);

                        SoundEngine.PlaySound(RoRSound.PlantHeal.WithVolumeScale(0.25f).WithPitchVariance(0.5f), player.Center);
                    }
                }
            }
        }
    }
}
