using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;

namespace RoRMod.Content.Buffs
{
    internal class SnakeEyesBuff : TimedBuff
    {
        public SnakeEyesBuff(Entity entity, int duration) : base(entity, duration) 
        {
            SoundEngine.PlaySound(RoRSound.SnakeEyes.WithVolumeScale(0.5f), Entity.Center);
        }
    }
}
