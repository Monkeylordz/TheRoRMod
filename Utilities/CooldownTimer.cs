using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoRMod.Utilities
{
    public class CooldownTimer
    {
        public enum ResetMode { Automatic, Manual }

        public bool OnCooldown => CurrentTime > 0;
        public bool OffCooldown => !OnCooldown;
        public int CurrentTime { get; private set; }
        public int MaxTime { get; private set; }
        public int TotalTicksOnCooldown { get; private set; }
        public int TotalTicksOffCooldown { get; private set; }
        public ResetMode resetMode { get; private set; }

        public CooldownTimer(ResetMode resetMode) : this(resetMode, 0, 0) { }

        public CooldownTimer(ResetMode resetMode, int maxTime) : this(resetMode, maxTime, maxTime) { }

        public CooldownTimer(ResetMode resetMode, int maxTime, int startingTime)
        {
            MaxTime = maxTime;
            this.resetMode = resetMode;
            SetTimer(startingTime);
        }

        public void Tick()
        {
            if (OnCooldown)
            {
                CurrentTime--;
                TotalTicksOnCooldown++;
                TotalTicksOffCooldown = 0;
            }
            else
            {
                if (resetMode == ResetMode.Automatic)
                {
                    ResetToMax();
                }
                TotalTicksOnCooldown = 0;
                TotalTicksOffCooldown++;
            }
        }

        public void ResetToZero()
        {
            CurrentTime = 0;
        }

        public void ResetToMax()
        {
            CurrentTime = MaxTime;
        }

        public void SetTimer(int time)
        {
            CurrentTime = time;
        }
    }
}
