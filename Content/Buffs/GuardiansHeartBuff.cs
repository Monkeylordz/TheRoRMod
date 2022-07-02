using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;

namespace RoRMod.Content.Buffs
{
    internal class GuardiansHeartBuff : Buff
    {
        public override bool Active => true;

        public GuardiansHeartBuff(Entity entity) : base(entity) 
        {
            SoundEngine.PlaySound(RoRSound.ShieldUp.WithVolumeScale(0.5f), Entity.Center);
        }
    }
}
