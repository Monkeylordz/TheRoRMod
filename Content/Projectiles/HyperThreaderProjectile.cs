using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using System;
using Terraria.DataStructures;
using Terraria.Audio;

namespace RoRMod.Content.Projectiles
{
    internal class HyperThreaderProjectile : ModProjectile
    {
		public const float DetectionRadius = 400f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hyper-Threaded Laser");

            // Cultist resistance to homing projectiles
            // ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = 0;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = false;
			Projectile.light = 1f;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 300;
			Projectile.maxPenetrate = 4;
			Projectile.penetrate = 4;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.penetrate <= 0)
            {
				Projectile.Kill();
            }
			Projectile.penetrate--;

			// Bounce on tiles
			if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}
			Projectile.rotation = Projectile.velocity.ToRotation();

			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			// On Hit: bounce to new target
			NPC npc = NPCUtils.GetClosestNPC(Projectile.Center, NPCConditions.Base.CanBeChased().WithinRadius(Projectile.Center, DetectionRadius).Exclude(new int[] { target.whoAmI }));
			if (npc != null)
			{
				Projectile.velocity = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Items.Weapons.HyperThreader.HyperLaserSpeed;
				Projectile.rotation = Projectile.velocity.ToRotation();
			}
		}

        public override void OnSpawn(IEntitySource source)
        {
			// Increase penetrate if shot by item
			if (typeof(EntitySource_ItemUse_WithAmmo).IsInstanceOfType(source))
            {
				Projectile.maxPenetrate = 10;
				Projectile.penetrate = 10;
            }

			Projectile.rotation = Projectile.velocity.ToRotation();
		}

        public override void AI()
        {
            // Dust
            if (Main.rand.NextBool(5))
            {
				Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.RedTorch);
            }
		}
	}
}
