using Terraria;
using Terraria.ID;
using Terraria.Audio;
using RoRMod.Content.Common;

namespace RoRMod.Content.Buffs
{
    internal class FilialAttackSpeedBuff : Buff
    {
        public override bool Active => true;

        public FilialAttackSpeedBuff(Entity entity) : base(entity) 
        {
            SoundEngine.PlaySound(RoRSound.FilialAttack.WithVolumeScale(0.5f), Entity.Center);
        }

        public override void Update()
        {
            base.Update();
            // Dust
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Entity.TopLeft, Entity.width, Entity.height, DustID.RedTorch);
            }
        }
    }
}
