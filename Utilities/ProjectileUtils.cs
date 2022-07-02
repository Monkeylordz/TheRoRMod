using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace RoRMod.Utilities
{
	internal static class ProjectileUtils
    {
		/// <summary>
		/// Creates an Explosion Projectile with the given parameters.
		/// </summary>
		/// <param name="player">The owner of the projectile</param>
		/// <param name="spawnSource">The IEntitySource</param>
		/// <param name="position">The position of the explosion center</param>
		/// <param name="radius">The radius of the explosion</param>
		/// <param name="damage">The damage of the explosion</param>
		/// <param name="knockback">The knockback of the explosion</param>
		/// <param name="delay">The delay before the explosion occurs</param>
		/// <param name="projectileType">The projectile type used as the explosion. Leave as null to use the default explosion projectile. </param>
		/// <returns>The explosion's projectile id</returns>
		public static int CreateExplosion(Player player, IEntitySource spawnSource, Vector2 position, float radius, int damage, float knockback, int delay = 0, int? projectileType = null)
        {
			if (projectileType == null)
			{
				projectileType = ModContent.ProjectileType<Content.Projectiles.ExplosionProjectile>();
			}

			return Projectile.NewProjectile(spawnSource, position, Vector2.Zero, projectileType.Value,
				damage, knockback, player.whoAmI, radius, delay);
        }

		/// <summary>
		/// Spawns projectiles to damage NPCs.
		/// </summary>
		/// <param name="player">The owner of the projectiles</param>
		/// <param name="spawnSource">The IEntitySource</param>
		/// <param name="npcs">The NPCs to damage</param>
		/// <param name="damage">The amount of damage</param>
		/// <param name="knockback">The amount of knockback</param>
		/// <param name="projectileType">The projectile type used to damage the NPCS. Leave as null to use the default damage instance projectile.</param>
		public static void DamageNPCs(Player player, IEntitySource spawnSource, IEnumerable<NPC> npcs, int damage, float knockback, int? projectileType = null)
        {
			if (projectileType == null)
            {
				projectileType = ModContent.ProjectileType<Content.Projectiles.DamageProjectile>();
			}

			foreach (NPC npc in npcs)
            {
				Projectile.NewProjectile(spawnSource, npc.Center, Vector2.Zero, projectileType.Value,
					damage, knockback, player.whoAmI, ai0: npc.whoAmI);
            }
        }

		/// <summary>
		/// Rotates a vector to align towards a target position.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="vector"></param>
		/// <param name="targetPosition"></param>
		/// <param name="maxRotation"></param>
		/// <returns></returns>
		public static Vector2 AlignVector(Vector2 vector, Vector2 position, Vector2 targetPosition, float maxRotation = MathHelper.TwoPi)
        {
			Vector2 targetDirection = targetPosition - position;
			float currentAngle = MathF.Atan2(vector.X, vector.Y);
			float targetAngle = MathF.Atan2(targetDirection.X, targetDirection.Y);
			float angleDiff =  currentAngle - targetAngle;
			if (MathF.Abs(angleDiff) > maxRotation)
            {
				return vector.RotatedBy(MathF.Sign(angleDiff) * maxRotation);
            }
			return vector.RotatedBy(angleDiff);
		}

		/// <summary>
		/// Calculates the direction for a projectile to move in order to hit a target moving at a constant velocity
		/// </summary>k
		/// <param name="position">The current position of the projectile</param>
		/// <param name="targetPosition">The current position of the target</param>
		/// <param name="targetVelocity">The current velocity of the target</param>
		/// <param name="speed">The speed of the projectile</param>
		/// <param name="acceleration">The acceleration of the projectile</param>
		/// <returns>The direction in which the projectile should move</returns>
		public static Vector2 PredictDirectionToMovingTarget(Vector2 projectilePosition, Vector2 targetPosition, Vector2 targetVelocity, float speed, float acceleration)
        {
			// K(t): the position of the target after time t
			Vector2 K(float t) => targetPosition + targetVelocity * t;
			// D(t): the direction from the projectile to the target at time t
			Vector2 D(float t) =>  K(t) - projectilePosition;
			// Dn(t): D(t) normalized
			Vector2 Dn(float t) => D(t).SafeNormalize(Vector2.Zero);
			// P(t): the position of the projectile after time t
			Vector2 P(float t) => projectilePosition + Dn(t) * speed * t + Dn(t) * acceleration * t * t;
			// Kdt(t): the derivative of K(t) wrt t
			Vector2 Kdt(float t) => targetVelocity;
			// Ddt(t): the derivative of D(t) wrt t
			Vector2 Ddt(float t) => targetVelocity;
			// Dndt(t): the derivative of Dn(t) wrt t
			Vector2 Dndt(float t) => (Ddt(t) - Dn(t) * Vector2.Dot(Dn(t), Ddt(t))) / D(t).Length();
			// Pdt(t): the derivative of P(t) wrt t
			Vector2 Pdt(float t) => Dn(t) * (speed + 2 * acceleration * t) + Dndt(t) * (speed * t + acceleration * t * t);
			// X(t): the distance between the projectile and the target at time t
			// float X(float t) => Vector2.DistanceSquared(K(t), P(t)) / 2;
			// Xdt(t): the derivative of X wrt t
			float Xdt(float t) => Vector2.Dot(K(t) - P(t), Kdt(t) - Pdt(t));

			// Use Secant Method to find t that minimizes X(t), which occurs when gX(t) = 0
			float t = SecantMethod(Xdt);

			return Dn(t);
        }

		delegate float function(float x);
		private static float SecantMethod(function f, int maxItn = 100, float threshold = 0.001f)
        {
			// Secant Method fis an iterative method that approximates the root of a given function
			int itn = 0;
			float xn = 0, xn1 = 100, xn2 = 99;
			while (itn < maxItn)
			{
				// Secant step
				xn = xn1 - f(xn1) * (xn1 - xn2) / (f(xn1) - f(xn2));

				// Threshold
				if (MathF.Abs(xn - xn1) < threshold) break;

				// Update
				xn2 = xn1;
				xn1 = xn;

				itn++;
			}

			if (float.IsNaN(xn)) xn = 0;

			return xn;
		}

		/// <summary>
		/// Clamps a Vector2 based on its magnitude
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns>The clamped vector</returns>
		public static Vector2 ClampMagnitude(Vector2 vector, float min, float max)
        {
			float sqrMagnitude = vector.LengthSquared();
			if (sqrMagnitude < min * min)
            {
				return vector.SafeNormalize(Vector2.Zero) * min;
			}
			else if (sqrMagnitude > max * max)
            {
				return vector.SafeNormalize(Vector2.Zero) * max;
			}
			return vector;
		}

		public static bool IsRectangleWithinRadius(Vector2 point, float radius, Rectangle rect)
        {
			return Vector2.DistanceSquared(point, rect.ClosestPointInRect(point)) <= radius * radius;
		}

		/// <summary>
		/// Creates a Rectangle from two of its corners
		/// </summary>
		/// <param name="corner1"></param>
		/// <param name="corner2"></param>
		/// <returns></returns>
		public static Rectangle RectangleFromCorners(Vector2 corner1, Vector2 corner2)
		{
			if (corner1.X < corner2.X)
			{
				if (corner1.Y < corner2.Y)
				{
					// corner1 top left, corner2 bottom right
					return new Rectangle((int)corner1.X, (int)corner1.Y, (int)(corner2.X - corner1.X), (int)(corner2.Y - corner1.Y));
				}
				else
				{
					// corner1 bottom left, corner2 top right
					return new Rectangle((int)corner1.X, (int)corner2.Y, (int)(corner2.X - corner1.X), (int)(corner1.Y - corner2.Y));
				}
			}
			else
			{
				if (corner1.Y < corner2.Y)
				{
					// corner1 top right, corner2 bottom left
					return new Rectangle((int)corner2.X, (int)corner1.Y, (int)(corner1.X - corner2.X), (int)(corner2.Y - corner1.Y));
				}
				else
				{
					// corner1 bottom right, corner2 top left
					return new Rectangle((int)corner2.X, (int)corner2.Y, (int)(corner1.X - corner2.X), (int)(corner1.Y - corner2.Y));
				}
			}
		}
	}
}
