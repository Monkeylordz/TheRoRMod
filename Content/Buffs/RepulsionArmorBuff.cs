using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;

namespace RoRMod.Content.Buffs
{
    internal class RepulsionArmorBuff : TimedBuff
    {
        public RepulsionArmorBuff(Entity entity, int duration) : base(entity, duration) 
        {
            SoundEngine.PlaySound(RoRSound.RepulsionArmorActivate.WithVolumeScale(0.5f), Entity.Center);
        }
    }
}
