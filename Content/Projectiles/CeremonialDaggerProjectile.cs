using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using System;
using Terraria.DataStructures;
using RoRMod.Content.Items.Weapons;
using Terraria.Audio;
using RoRMod.Content.Common;

namespace RoRMod.Content.Projectiles
{
    internal class CeremonialDaggerProjectile : ModProjectile
    {
		public const float DetectionRadius = 400f;
		public const int WaitTime = 15;

		public enum Phase { Wait, Search, None }

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
            DisplayName.SetDefault("Ceremonial Dagger");
        }

		public override void SetDefaults()
		{
			Projectile.width = 36; 
			Projectile.height = 16;
			Projectile.aiStyle = 0;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = false;
			Projectile.light = 0.5f;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 600;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			return base.OnTileCollide(oldVelocity);
        }

        public override void OnSpawn(IEntitySource source)
        {
			UpdateRotation();
			AIPhase = Phase.Wait;

			SoundEngine.PlaySound(RoRSound.DaggerFly.WithVolumeScale(0.5f), Projectile.Center);
		}

        public override void AI()
        {
			// Wait, search for target, then launch toward target
			switch (AIPhase)
			{
				case Phase.Wait:
					WaitPhase();
					break;
				case Phase.Search:
					SearchPhase();
					break;
			}

			// Dust
			if (Main.rand.NextBool(5))
            {
				for (int i = 0; i < 3; i++)
                {
					int d1 = Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.Shadowflame, SpeedY: -1f);
					Main.dust[d1].noGravity = true;
				}
			}

			Timer++;
		}

		// Wait Phase: Wait before searching, no gravity or drag
		private void WaitPhase()
		{
			UpdateRotation();

			if (Timer >= WaitTime)
			{
				AIPhase = Phase.Search;
			}
		}

		// Search Phase: Search for target, then launch toward it
		private void SearchPhase()
		{
			GravityAndDrag();

			// Search every 10 frames
			if (Timer % 10 == 0)
			{
				NPC closestNPC = NPCUtils.GetClosestNPC(Projectile.Center, NPCConditions.Base.CanBeChased().WithinRadius(Projectile.Center, DetectionRadius));
				if (closestNPC != null)
                {
					// Launch toward target
					Vector2 direction = ProjectileUtils.PredictDirectionToMovingTarget(Projectile.Center, closestNPC.Center, closestNPC.velocity, CeremonialDagger.DaggerSpeed, 0);
					Projectile.velocity = direction * CeremonialDagger.DaggerSpeed;
					UpdateRotation();
					AIPhase = Phase.None;

					SoundEngine.PlaySound(RoRSound.DaggerFly.WithVolumeScale(0.5f), Projectile.Center);
				}
			}
		}

		private void GravityAndDrag()
        {
			Projectile.velocity.Y = MathF.Min(Projectile.velocity.Y + 0.2f, 40);
			Projectile.velocity.X *= 0.99f;

			UpdateRotation();
		}

		private void UpdateRotation()
        {
			Projectile.spriteDirection = Projectile.direction;
			Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : -MathHelper.Pi);
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			SoundEngine.PlaySound(RoRSound.DaggerHit.WithVolumeScale(0.3f), Projectile.Center);
        }
    }
}
