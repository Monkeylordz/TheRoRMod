using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using System.IO;
using Terraria.DataStructures;
using RoRMod.Content.Items.Weapons;
using Terraria.Audio;

namespace RoRMod.Content.Projectiles
{
    internal class NematocystProjectile : ModProjectile
    {
        public const float DetectionRadius = 400f;
        public const float Acceleration = 0.4f;
        public const float MaxSpeed = 200f;
        public const int StoppingTime = 60;
        public const int MinStartingTime = 0;
        public const int MaxStartingTime = 15;

        public enum Phase { Slowdown, Search, Acceleration }

        private int Timer
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        private Phase AIPhase
        {
            get { return (Phase)Projectile.ai[1]; }
            set { Projectile.ai[1] = (float)value; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nematocyst");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Timer = Main.rand.Next(MinStartingTime, MaxStartingTime);
            Projectile.rotation = Projectile.velocity.ToRotation();
            AIPhase = Phase.Slowdown;
        }

        public override void AI()
        {
            // Burst out fast, slow down, then accelerate to nearest npc
            switch (AIPhase)
            {
                case Phase.Slowdown:
                    SlowdownPhase();
                    break;
                case Phase.Search:
                    SearchPhase();
                    break;
                case Phase.Acceleration:
                    AcceleratePhase();
                    break;
            }

            // Dust, 5% chance
            if (Main.rand.NextBool(5))
            {
                int d1 = Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.BubbleBurst_Blue);
                Main.dust[d1].noGravity = true;
            }
        }

        // Slow Phase: Slow to stop within 60 frames
        private void SlowdownPhase()
        {
            float progress = MathHelper.Clamp((float)Timer / StoppingTime, 0, 1);
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * NematocystNozzle.NematocystSpeed * (1 - progress);
            Timer++;

            if (progress >= 1)
            {
                Timer = 0;
                AIPhase = Phase.Search;
            }
        }

        // Search Phase: Search for NPC
        private void SearchPhase()
        {
            // Search every 10 frames
            if (Timer % 10 == 0)
            {
                NPC target = NPCUtils.GetClosestNPC(Projectile.Center, NPCConditions.Base.CanBeChased().WithinRadius(Projectile.Center, DetectionRadius));
                if (target != null)
                {
                    Vector2 direction = ProjectileUtils.PredictDirectionToMovingTarget(Projectile.Center, target.Center, target.velocity, Acceleration, Acceleration);
                    Projectile.velocity = direction * Acceleration;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    AIPhase = Phase.Acceleration;
                }
            }
            Timer++;
        }

        // Acceleration Phase: Accelerate in target direction
        private void AcceleratePhase()
        {
            Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * Acceleration;
            Projectile.velocity = ProjectileUtils.ClampMagnitude(Projectile.velocity, 0, MaxSpeed);
        }

        public override void Kill(int timeLeft)
        {
            Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.BubbleBlock);
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);
        }
    }
}
