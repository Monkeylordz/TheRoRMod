using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace RoRMod.Content.Buffs
{
    internal class FreezeBuff : TimedBuff
    {
        public FreezeBuff(Entity entity, int duration) : base(entity, duration) 
        {
            Entity.velocity = Vector2.Zero;
            SoundEngine.PlaySound(RoRSound.Freeze.WithVolumeScale(0.4f), Entity.Center);
        }

        public override void Update()
        {
            base.Update();
            // Dust
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Entity.TopLeft, Entity.width, Entity.height, DustID.IceTorch);
            }
        }
    }
}
