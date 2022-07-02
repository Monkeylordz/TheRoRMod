using Terraria;
using Terraria.ID;
using System;
using Terraria.Audio;
using RoRMod.Content.Common;

namespace RoRMod.Content.Buffs
{
    internal class PoisonBuff : DamagingBuff
    {
		public PoisonBuff(IDamageable buffedEntity, int damagePerSecond, int duration) : base(buffedEntity, damagePerSecond, duration, 30) 
		{
			SoundEngine.PlaySound(RoRSound.Poison.WithVolumeScale(0.5f), Entity.Center);
		}

		protected override int ModifyDamagePerInterval(int damage)
		{
			return Math.Max(base.ModifyDamagePerInterval((int)(buffedEntity.MaxLife * 0.01f)), base.ModifyDamagePerInterval(damage));
		}

		public override void Update()
		{
			base.Update();

			// Dust
			if (Main.rand.NextBool(8))
			{
				Dust.NewDust(Entity.TopLeft, Entity.width, Entity.height, DustID.Poisoned);
			}
		}
	}
}
