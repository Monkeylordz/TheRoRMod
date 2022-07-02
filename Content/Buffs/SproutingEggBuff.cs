using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;

namespace RoRMod.Content.Buffs
{
    internal class SproutingEggBuff : Buff
    {
        public override bool Active => true;

        public SproutingEggBuff(Entity entity) : base(entity) 
        {
            SoundEngine.PlaySound(RoRSound.EggStart.WithVolumeScale(0.5f), Entity.Center);
        }
    }
}
