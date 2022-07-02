using RoRMod.Utilities;
using Terraria;

namespace RoRMod.Content.Buffs
{
    internal abstract class TimedBuff : Buff
    {
        public override bool Active => BuffTimer.OnCooldown;

        /// <summary>
        /// The timer of the buff.
        /// </summary>
        protected CooldownTimer BuffTimer { get; private set; }

        public TimedBuff(Entity entity) : base(entity)
        {
            BuffTimer = new CooldownTimer(CooldownTimer.ResetMode.Manual);
        }

        public TimedBuff(Entity entity, int duration) : base(entity)
        {
            BuffTimer = new CooldownTimer(CooldownTimer.ResetMode.Manual, duration);
        }

        public override void Update()
        {
            if (BuffTimer.OnCooldown)
            {
                BuffTimer.Tick();
            }
        }

        public override void Clear()
        {
            BuffTimer.ResetToZero();
        }
    }
}
