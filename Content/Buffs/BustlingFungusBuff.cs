using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;

namespace RoRMod.Content.Buffs
{
    internal class BustlingFungusBuff : Buff
    {
        public override bool Active => true;

        public BustlingFungusBuff(Entity entity) : base(entity) 
        {
            SoundEngine.PlaySound(RoRSound.BustlingFungusStart.WithVolumeScale(0.25f), Entity.Center);
        }

        public override void Clear()
        {
            SoundEngine.PlaySound(RoRSound.BustlingFungusStop.WithVolumeScale(0.25f), Entity.Center);
        }
    }
}
