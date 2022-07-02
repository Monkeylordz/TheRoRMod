using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace RoRMod.Content.Buffs
{
    internal class RedWhipBuff : Buff
    {
        public override bool Active => true;

        public RedWhipBuff(Entity entity) : base(entity) 
        {
            SoundEngine.PlaySound(RoRSound.RedWhip.WithVolumeScale(0.75f), Entity.Center);
            for (int i = 0; i < 15; i++)
                Dust.NewDust(Entity.Bottom, Entity.width, 0, DustID.Cloud, SpeedX: Main.rand.NextFloat(-2, 2));
        }
    }
}
