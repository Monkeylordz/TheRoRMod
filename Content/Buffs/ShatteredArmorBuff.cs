using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace RoRMod.Content.Buffs
{
    internal class ShatteredArmorBuff : TimedBuff
	{
		public int Stacks { get; private set; } = 0;
		private IDamageable buffedEntity;

		public ShatteredArmorBuff(IDamageable buffedEntity, int duration) : base(buffedEntity.Entity) 
		{
			this.buffedEntity = buffedEntity;
			Apply(duration);
		}

		public void Apply(int duration)
		{
			Stacks++;
			BuffTimer.SetTimer(duration);

			// Dust
			for (int i = 0; i < 5; i++)
			{
				float speedX = Main.rand.NextFloat(-1, 1);
				float speedY = Main.rand.NextFloat(0.5f);
				Dust.NewDust(Entity.TopLeft, Entity.width, Entity.height, DustID.Tin, speedX, speedY);
			}

			SoundEngine.PlaySound(RoRSound.ShieldBreak.WithVolumeScale(0.75f), Entity.Center);
		}

		public override void Update()
		{
			base.Update();
			buffedEntity.Defense -= 4 * Stacks;
			buffedEntity.Defense = buffedEntity.Defense > 0 ? buffedEntity.Defense : 0;
		}

        public override void Draw(Vector2 position)
        {
            base.Draw(position);

			// Draw stack count
			DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, Stacks.ToString(), position - Main.screenPosition, Color.White);
		}
    }
}
