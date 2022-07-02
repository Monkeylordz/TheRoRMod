using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace RoRMod.Content.Buffs
{
    internal class BleedBuff : DamagingBuff
    {
		public BleedBuff(IDamageable buffedEntity, int damagePerSecond, int duration) : base(buffedEntity, damagePerSecond, duration, 15) 
		{
			SoundEngine.PlaySound(RoRSound.Bleed.WithVolumeScale(0.33f), Entity.Center);
		}

		public override void Update()
		{
			base.Update();

			// Dust
			if (Main.rand.NextBool(8))
			{
				Dust.NewDust(Entity.TopLeft, Entity.width, Entity.height, DustID.Blood);
			}
		}
	}
}
