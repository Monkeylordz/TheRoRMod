using Terraria;
using ReLogic.Graphics;
using Terraria.GameContent;
using System;

using Microsoft.Xna.Framework;

namespace RoRMod.Content.Buffs
{
    internal class ExtraLifeCooldownBuff : TimedBuff
    {
        public ExtraLifeCooldownBuff(Entity entity, int duration) : base(entity, duration) { }

        public override void Draw(Vector2 position)
        {
            base.Draw(position);

            // Show time left
            int sec = BuffTimer.CurrentTime / 60;
            int min = sec / 60;
            sec %= 60;
            TimeSpan time = new TimeSpan(0, min, sec);
            position += new Vector2(-14, 6);
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, time.ToString(@"mm\:ss"), 
                position - Main.screenPosition, Color.White, 0, Vector2.Zero, 0.5f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
        }
    }
}
