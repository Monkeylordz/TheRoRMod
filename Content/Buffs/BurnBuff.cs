using Terraria;
using Terraria.ID;

namespace RoRMod.Content.Buffs
{
    internal class BurnBuff : DamagingBuff
    {
		public BurnBuff(IDamageable buffedEntity, int damagePerSecond, int duration) : base(buffedEntity, damagePerSecond, duration, 15) 
		{ 
			// TODO: Sound
		}

		public override void Update()
		{
			base.Update();

			// Dust
			if (Main.rand.NextBool(5))
			{
				Dust.NewDust(Entity.TopLeft, Entity.width, Entity.height, DustID.Torch, SpeedY: 0.5f);
			}
		}
    }
}
