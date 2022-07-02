using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Content.Items.Accessories;
using RoRMod.Utilities;
using System;
using Terraria.Audio;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Projectiles
{
    internal class ConcussionGrenadeProjectile : ModProjectile
    {
        private enum Phase { Default, Exploding }

        private float RotationSpeed
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private Phase AIPhase
        {
            get => (Phase)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Concussion Grenade");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 20;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Exploding: collision within radius
            if (AIPhase == Phase.Exploding)
            {
                return ProjectileUtils.IsRectangleWithinRadius(Projectile.Center, ConcussionGrenade.BlastRadius, targetHitbox);
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
            if (AIPhase != Phase.Exploding)
            {
                Explode();
            }
            // Stun debuff
            target.GetGlobalNPC<BuffableNPC>().Buffs.AddBuff(new StunBuff(target, ConcussionGrenade.StunDuration));
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            RotationSpeed = Main.rand.NextFloat(-0.15f, 0.15f);
        }

        public override void AI()
        {
            if (AIPhase == Phase.Exploding)
            {
                return;
            }

            // Spin
            Projectile.rotation += RotationSpeed;

            // Gravity
            Projectile.velocity.Y = MathF.Min(Projectile.velocity.Y + 0.2f, 40);

            // Explode on time up
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft < 5)
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

            // Dust - Sparkles
            for (int i = 0; i < 20; i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(1, 1) * ConcussionGrenade.BlastRadius;
                Dust.NewDustPerfect(position, DustID.TreasureSparkle, Vector2.Zero, Scale: 0.5f + Main.rand.NextFloat());
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
    }
}
