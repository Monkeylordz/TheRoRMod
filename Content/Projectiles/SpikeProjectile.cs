using Microsoft.Xna.Framework;
using RoRMod.Content.Buffs;
using RoRMod.Content.Common;
using RoRMod.Content.Items.Accessories;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class SpikeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void AI()
        {
            // Gravity and Drag
            Projectile.velocity.Y = MathF.Min(Projectile.velocity.Y + 0.3f, 30);
            Projectile.velocity.X *= 0.97f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Slow target by 10% for 0.5 seconds
            target.GetGlobalNPC<BuffableNPC>().Buffs.AddBuff(new SlowBuff(target, Spikestrip.SlowDuration, Spikestrip.SlowAmount));
        }
    }
}
