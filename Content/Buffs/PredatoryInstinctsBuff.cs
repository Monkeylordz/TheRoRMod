using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using RoRMod.Content.Items.Accessories;
using Terraria.Audio;
using RoRMod.Content.Common;

namespace RoRMod.Content.Buffs
{
    internal class PredatoryInstinctsBuff : TimedBuff
    {
		public int Stacks { get; private set; } = 0;

		public PredatoryInstinctsBuff(Entity entity, int duration) : base(entity) 
        {
            Apply(duration);
        }

		public void Apply(int duration)
		{
			if (Stacks < PredatoryInstincts.MaxStacks)
            {
				Stacks++;
				BuffTimer.SetTimer(duration);

                // Play sound corresponding to stack count
                SoundStyle soundStyle;
                if (Stacks == 1)
                {
                    soundStyle = RoRSound.CritAttackSpeed1;
                }
                else if (Stacks == 2)
                {
                    soundStyle = RoRSound.CritAttackSpeed2;
                }
                else
                {
                    soundStyle = RoRSound.CritAttackSpeed3;
                }
                SoundEngine.PlaySound(soundStyle.WithVolumeScale(0.3f), Entity.Center);
            }
		}

        public override void Update()
        {
            base.Update();

			// Play sound when the buff wears off
			if (!Active && BuffTimer.TotalTicksOffCooldown == 0)
            {
				SoundEngine.PlaySound(RoRSound.CritAttackSpeedCooldown.WithVolumeScale(0.3f), Entity.Center);
			}
        }

        public override void Draw(Vector2 position)
        {
            base.Draw(position);

			// Draw stack count
			DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, Stacks.ToString(), position - Main.screenPosition, Color.White);
		}
    }
}
