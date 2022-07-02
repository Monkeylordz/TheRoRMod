using Microsoft.Xna.Framework;
using RoRMod.Content.Buffs;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class FiretrailProjectile : ModProjectile
    {
        public const int BurnDuration = 120;
        public const int FrameCount = 4;
        public const int AnimationDelay = 8;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Firetrail");

            Main.projFrames[Projectile.type] = FrameCount;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.scale = 0.75f;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = false;
            Projectile.light = 1f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 90;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            BuffableNPC buffableNPC = target.GetGlobalNPC<BuffableNPC>();
            buffableNPC.Buffs.AddBuff(new BurnBuff(buffableNPC, damage, BurnDuration));
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity.X = Main.rand.NextFloat(-0.3f, 0.3f);
        }

        public override void AI()
        {
            // Extinguish in water
            if (Projectile.wet)
            {
                Projectile.Kill();
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.Smoke);
                }
            }

            // Animate
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationDelay)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                Projectile.frame %= FrameCount;
            }

            // Dust
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.Torch);
            }
        }
    }
}
