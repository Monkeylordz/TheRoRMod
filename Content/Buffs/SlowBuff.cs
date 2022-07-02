using RoRMod.Content.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace RoRMod.Content.Buffs
{
    internal class SlowBuff : TimedBuff
    {
        private const float slowAmountMultiplier = 0.5f;

        private float slowAmount;

        public SlowBuff(Entity entity, int duration, float slowAmount) : base(entity, duration) 
        {
            this.slowAmount = slowAmount;
            SoundEngine.PlaySound(RoRSound.Slow.WithVolumeScale(0.25f), Entity.Center);
        }

        public override void Update()
        {
            base.Update();

            // Slow
            Entity.velocity *= 1 - slowAmount * slowAmountMultiplier;

            // Dust
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Entity.TopLeft, Entity.width, Entity.height, DustID.TreasureSparkle);
            }
        }
    }
}
