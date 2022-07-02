using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Items.Weapons;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Projectiles
{
    internal class SawmerangProjectile : ModProjectile
    {
		public const float BleedMultiplier = 0.7f;
		public const int BleedDuration = 120;

		public enum Phase { Throw, Return }

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
            DisplayName.SetDefault("Sawmerang");
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.scale = 0.75f;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 900;
            Projectile.maxPenetrate = 10;
			Projectile.penetrate = 10;

			DrawOffsetX = -(int)((1 - Projectile.scale) * Projectile.width / 2);
			DrawOriginOffsetY = -(int)((1 - Projectile.scale) * Projectile.height / 2);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			// 25% chance to bleed
			if (PlayerUtils.ChanceRoll(Main.player[Projectile.owner], 0.25f))
            {
				BuffableNPC buffableNPC = target.GetGlobalNPC<BuffableNPC>();
				buffableNPC.Buffs.AddBuff(new BleedBuff(buffableNPC, (int)(BleedMultiplier * damage), BleedDuration));
			}

			SoundEngine.PlaySound(RoRSound.SawHit.WithVolumeScale(0.25f), Projectile.Center);
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Projectile.velocity = -oldVelocity;
			AIPhase = Phase.Return;
			Projectile.tileCollide = false;
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
			return false;
		}

		public override void AI()
		{
			switch(AIPhase)
            {
				case Phase.Throw:
					Throw();
					break;
				case Phase.Return:
					Return();
					break;
			}

			// Rotation
			Projectile.rotation += 0.4f * Projectile.direction;

			// Sound
			if (Projectile.soundDelay == 0)
			{
				Projectile.soundDelay = 8;
				SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);
			}

			Timer++;
		}

		private void Throw()
		{
			if (Timer > Sawmerang.TravelTime)
			{
				Projectile.tileCollide = false;
				AIPhase = Phase.Return;
			}
		}

		private void Return()
		{
			if (Timer % 3 == 0)
            {
				Vector2 returnDirection = Main.player[Projectile.owner].Center - Projectile.Center;

				// Kill if too far away
				if (returnDirection.LengthSquared() > 3000 * 3000)
				{
					Projectile.Kill();
				}

				// Move towards player
				returnDirection = returnDirection.SafeNormalize(Vector2.Zero);
				Projectile.velocity += returnDirection * Sawmerang.ReturnAcceleration;

				// Extra boost if moving away from player in a direction
				if (Projectile.velocity.X * returnDirection.X < 0)
                {
					Projectile.velocity.X += returnDirection.X * Sawmerang.ReturnAcceleration;
				}
				if (Projectile.velocity.Y * returnDirection.Y < 0)
				{
					Projectile.velocity.Y += returnDirection.Y * Sawmerang.ReturnAcceleration;
				}

				// Clamp Speed
				Projectile.velocity = ProjectileUtils.ClampMagnitude(Projectile.velocity, 0, Sawmerang.ReturnSpeed);

				// Kill if returned to player
				if (Main.myPlayer == Projectile.owner)
				{
					if (Projectile.Hitbox.Intersects(Main.player[Projectile.owner].Hitbox))
					{
						Projectile.Kill();
					}
				}
			}
		}
	}
}
