using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using System;
using System.IO;
using Terraria.DataStructures;
using ReLogic.Utilities;
using Terraria.Audio;
using RoRMod.Content.Common;

namespace RoRMod.Content.Projectiles
{
    internal class MissileProjectile : ModProjectile
    {
		public const float Speed = 9f;
		public const float DetectionRadius = 1000f;
		public const float RotationSpeed = MathHelper.Pi / 32;

		private const int FlySoundDelay = 60;
		private SlotId flySoundSlotId = SlotId.Invalid;

		public enum Phase { Wait, Wander, Homing }

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

		private float RotationGoal { get; set; }
		private int TargetNPC { get; set; }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(RotationGoal);
			writer.Write(TargetNPC);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			RotationGoal = reader.ReadSingle();
			TargetNPC = reader.ReadInt32();
		}

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Missile");

            // Cultist resistance to homing projectiles
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = 0;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.5f;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
		}

        public override void OnSpawn(IEntitySource source)
        {
			Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY) * Speed;
			Projectile.rotation = Projectile.velocity.ToRotation();
			AIPhase = Phase.Wait;
		}

        public override void AI()
		{
			// Launch upwards, then home in to nearest npc, otherwise wander
			switch (AIPhase)
			{
				case Phase.Wait:
					WaitPhase();
					break;
				case Phase.Wander:
					WanderPhase();
					break;
				case Phase.Homing:
					HomingPhase();
					break;
			}

			// Dust
			if (Main.rand.NextBool(5))
            {
				Vector2 flareVelocity = -Vector2.Normalize(Projectile.velocity) * 2f;
				Dust d1 = Dust.NewDustDirect(Projectile.Left, 0, Projectile.height, DustID.Flare, SpeedX: flareVelocity.X, SpeedY: flareVelocity.Y);
				d1.noGravity = true;
				Dust d2 = Dust.NewDustDirect(Projectile.Left, 0, Projectile.height, DustID.Smoke, Scale: 1.5f);
				d2.noGravity = true;
			}

            // Sound - flying
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = FlySoundDelay;
				flySoundSlotId = SoundEngine.PlaySound(RoRSound.MissileFly.WithVolumeScale(0.1f), Projectile.Center);
            }

            Timer++;
		}

		// Wait Phase
		private void WaitPhase()
        {
			if (Timer >= 5)
            {
				AIPhase = Phase.Wander;
            }
        }

		// Wander Phase: Wander while searching for an NPC
		private void WanderPhase()
        {
			// 1/15 chance/frame to change rotation goal
			if (Main.rand.NextBool(15))
			{
				RotationGoal += Main.rand.NextFloat(MathHelper.Pi) * (Main.rand.NextBool() ? 1 : -1);
			}

			// Turn toward rotation goal smoothly
			float rotationAmount = MathF.Min(MathF.Abs(RotationGoal), RotationSpeed) * Math.Sign(RotationGoal);
			RotationGoal -= rotationAmount;
			Projectile.velocity = Projectile.velocity.RotatedBy(rotationAmount);
			Projectile.rotation = Projectile.velocity.ToRotation();

			// Search for NPC
			if (Timer % 10 == 0)
			{
				NPC closestNPC = NPCUtils.GetClosestNPC(Projectile.Center, NPCConditions.Base.CanBeChased().WithinRadius(Projectile.Center, DetectionRadius));
				if (closestNPC != null)
				{
					TargetNPC = closestNPC.whoAmI;
					AIPhase = Phase.Homing;
				}
			}
		}

		// Phase 3: Home in on NPC
		private void HomingPhase()
        {
			// Check target NPC is still alive
			if (!Main.npc[TargetNPC].active)
			{
				AIPhase = Phase.Wander;
				return;
			}

			// Adjust direction smoothly
			Vector2 targetPosition = Main.npc[TargetNPC].Center;
			Projectile.velocity = ProjectileUtils.AlignVector(Projectile.velocity, Projectile.Center, targetPosition, RotationSpeed);
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

        public override void Kill(int timeLeft)
        {
			// Create Dust
			for (int i = 0; i < 10; i++)
			{
				Vector2 velocity = Main.rand.NextVector2Circular(5f, 5f);
				Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.DesertTorch, Velocity: velocity);
				dust.noGravity = true;
			}

			// Explosion sound
			SoundEngine.PlaySound(RoRSound.MissileExplode.WithVolumeScale(0.5f), Projectile.Center);

			// Stop flying sound
			if (SoundEngine.TryGetActiveSound(flySoundSlotId, out ActiveSound sound))
			{
				sound.Stop();
			}
		}
    }
}
