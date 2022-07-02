namespace RoRMod.Content.Buffs
{
	internal abstract class DamagingBuff : TimedBuff
	{
		protected IDamageable buffedEntity;
		protected int DamagePerInterval;
		protected int DamageInterval;

		/// <summary>
		/// A buff that repeatedly inflicts damages.
		/// </summary>
		/// <param name="buffedEntity">The buffed entity.</param>
		/// <param name="damagePerSecond">The damage per second of the buff.</param>
		/// <param name="duration">The duration of the buff.</param>
		/// <param name="damageInterval">The interval between two instances of damage in frames.</param>
		public DamagingBuff(IDamageable buffedEntity, int damagePerSecond, int duration, int damageInterval = 30) : base(buffedEntity.Entity, duration)
		{
			this.buffedEntity = buffedEntity;
			DamageInterval = damageInterval;
			DamagePerInterval = ModifyDamagePerInterval(damagePerSecond);
		}

		/// <summary>
		/// Determines the damage per interval when this buff is applied.
		/// </summary>
		/// <param name="damage">The damage that applied this buff.</param>
		/// <returns>The damage per interval.</returns>
		protected virtual int ModifyDamagePerInterval(int damage)
		{
			return (int)(damage / (60f / DamageInterval));
		}

		public override void Update()
		{
			base.Update();
			if (BuffTimer.TotalTicksOnCooldown % DamageInterval == 0)
			{
				buffedEntity.Damage(DamagePerInterval);
			}
		}
	}
}
