using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace RoRMod.Content.Buffs
{
    internal class BurningWitnessBuff : TimedBuff
    {
        public BurningWitnessBuff(Entity entity, int duration) : base(entity, duration) 
        {
            SoundEngine.PlaySound(RoRSound.BurningWitness.WithVolumeScale(0.25f));
        }

        public override void Update()
        {
            base.Update();

            // Dust
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Entity.TopLeft, Entity.width, Entity.height, DustID.GoldFlame);
            }
        }
    }
}
