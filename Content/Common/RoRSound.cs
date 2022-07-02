using Terraria.Audio;

namespace RoRMod.Content.Common
{
    public static class RoRSound
    {
        private const string PATH = "RoRMod/Assets/Sounds/";

        public static readonly SoundStyle Bear = new SoundStyle(PATH + nameof(Bear), 3);
        public static readonly SoundStyle BehemothBlast = new SoundStyle(PATH + nameof(BehemothBlast), 4);
        public static readonly SoundStyle BehemothChargeStock = new SoundStyle(PATH + nameof(BehemothChargeStock), 6);
        public static readonly SoundStyle Bleed = new SoundStyle(PATH + nameof(Bleed), 6);
        public static readonly SoundStyle BurningWitness = new SoundStyle(PATH + nameof(BurningWitness), 1);
        public static readonly SoundStyle BustlingFungusStart = new SoundStyle(PATH + nameof(BustlingFungusStart), 1);
        public static readonly SoundStyle BustlingFungusStop = new SoundStyle(PATH + nameof(BustlingFungusStop), 1);
        public static readonly SoundStyle ChainLightning = new SoundStyle(PATH + nameof(ChainLightning), 4);
        public static readonly SoundStyle ChargeDown = new SoundStyle(PATH + nameof(ChargeDown), 2);
        public static readonly SoundStyle ChargeUp = new SoundStyle(PATH + nameof(ChargeUp), 2);
        public static readonly SoundStyle ChargefieldLoop = new SoundStyle(PATH + nameof(ChargefieldLoop), 1);
        public static readonly SoundStyle ChargefieldStart = new SoundStyle(PATH + nameof(ChargefieldStart), 1);
        public static readonly SoundStyle ChargefieldStop = new SoundStyle(PATH + nameof(ChargefieldStop), 1);
        public static readonly SoundStyle ChestUnlockGolden = new SoundStyle(PATH + nameof(ChestUnlockGolden), 3);
        public static readonly SoundStyle ChestUnlockReinforced = new SoundStyle(PATH + nameof(ChestUnlockReinforced), 3);
        public static readonly SoundStyle ChestUnlockStandard = new SoundStyle(PATH + nameof(ChestUnlockStandard), 3);
        public static readonly SoundStyle ChestUnlockFail = new SoundStyle(PATH + nameof(ChestUnlockFail), 1);
        public static readonly SoundStyle CritAttackSpeed1 = new SoundStyle(PATH + nameof(CritAttackSpeed1), 1);
        public static readonly SoundStyle CritAttackSpeed2 = new SoundStyle(PATH + nameof(CritAttackSpeed2), 1);
        public static readonly SoundStyle CritAttackSpeed3 = new SoundStyle(PATH + nameof(CritAttackSpeed3), 1);
        public static readonly SoundStyle CritAttackSpeedCooldown = new SoundStyle(PATH + nameof(CritAttackSpeedCooldown), 1);
        public static readonly SoundStyle CritHeal = new SoundStyle(PATH + nameof(CritHeal), 5);
        public static readonly SoundStyle Crowbar = new SoundStyle(PATH + nameof(Crowbar), 2);
        public static readonly SoundStyle DaggerFly = new SoundStyle(PATH + nameof(DaggerFly), 4);
        public static readonly SoundStyle DaggerHit = new SoundStyle(PATH + nameof(DaggerHit), 4);
        public static readonly SoundStyle DaggerSpawn = new SoundStyle(PATH + nameof(DaggerSpawn), 6);
        public static readonly SoundStyle EggStart = new SoundStyle(PATH + nameof(EggStart), 1);
        public static readonly SoundStyle EggStop = new SoundStyle(PATH + nameof(EggStop), 1);
        public static readonly SoundStyle ExtraLife = new SoundStyle(PATH + nameof(ExtraLife), 3);
        public static readonly SoundStyle FilialAttack = new SoundStyle(PATH + nameof(FilialAttack), 1);
        public static readonly SoundStyle FilialRegen = new SoundStyle(PATH + nameof(FilialRegen), 1);
        public static readonly SoundStyle FilialSpeed = new SoundStyle(PATH + nameof(FilialSpeed), 1);
        public static readonly SoundStyle FireworkExplosion = new SoundStyle(PATH + nameof(FireworkExplosion), 7);
        public static readonly SoundStyle FireworkLaunch = new SoundStyle(PATH + nameof(FireworkLaunch), 10);
        public static readonly SoundStyle Fissure = new SoundStyle(PATH + nameof(Fissure), 4);
        public static readonly SoundStyle ForeignFruit = new SoundStyle(PATH + nameof(ForeignFruit), 4);
        public static readonly SoundStyle Freeze = new SoundStyle(PATH + nameof(Freeze), 4);
        public static readonly SoundStyle Gasoline = new SoundStyle(PATH + nameof(Gasoline), 3);
        public static readonly SoundStyle HitListKill = new SoundStyle(PATH + nameof(HitListKill), 4);
        public static readonly SoundStyle IceSwirl = new SoundStyle(PATH + nameof(IceSwirl), 5);
        public static readonly SoundStyle IceWind = new SoundStyle(PATH + nameof(IceWind), 6);
        public static readonly SoundStyle Infusion = new SoundStyle(PATH + nameof(Infusion), 6);
        public static readonly SoundStyle Instakill = new SoundStyle(PATH + nameof(Instakill), 5);
        public static readonly SoundStyle Lightning = new SoundStyle(PATH + nameof(Lightning), 3);
        public static readonly SoundStyle Medkit = new SoundStyle(PATH + nameof(Medkit), 1);
        public static readonly SoundStyle MineSpawn = new SoundStyle(PATH + nameof(MineSpawn), 1);
        public static readonly SoundStyle MissileExplode = new SoundStyle(PATH + nameof(MissileExplode), 4);
        public static readonly SoundStyle MissileFire = new SoundStyle(PATH + nameof(MissileFire), 4);
        public static readonly SoundStyle MissileFly = new SoundStyle(PATH + nameof(MissileFly), 4);
        public static readonly SoundStyle PlantGrow = new SoundStyle(PATH + nameof(PlantGrow), 1);
        public static readonly SoundStyle PlantHeal = new SoundStyle(PATH + nameof(PlantHeal), 1);
        public static readonly SoundStyle Poison = new SoundStyle(PATH + nameof(Poison), 1);
        public static readonly SoundStyle Punch = new SoundStyle(PATH + nameof(Punch), 2);
        public static readonly SoundStyle RedWhip = new SoundStyle(PATH + nameof(RedWhip), 4);
        public static readonly SoundStyle RepulsionArmorActivate = new SoundStyle(PATH + nameof(RepulsionArmorActivate), 2);
        public static readonly SoundStyle RepulsionArmorHit = new SoundStyle(PATH + nameof(RepulsionArmorHit), 4);
        public static readonly SoundStyle SawHit = new SoundStyle(PATH + nameof(SawHit), 3);
        public static readonly SoundStyle ShieldUp = new SoundStyle(PATH + nameof(ShieldUp), 3);
        public static readonly SoundStyle ShieldBreak = new SoundStyle(PATH + nameof(ShieldBreak), 2);
        public static readonly SoundStyle SnakeEyes = new SoundStyle(PATH + nameof(SnakeEyes), 3);
        public static readonly SoundStyle Slow = new SoundStyle(PATH + nameof(Slow), 3);
        public static readonly SoundStyle StickyBombActivate = new SoundStyle(PATH + nameof(StickyBombActivate), 3);
        public static readonly SoundStyle StickyBombCountdown = new SoundStyle(PATH + nameof(StickyBombCountdown), 1);
        public static readonly SoundStyle Stun = new SoundStyle(PATH + nameof(Stun), 6);
        public static readonly SoundStyle ToxicWormDeath = new SoundStyle(PATH + nameof(ToxicWormDeath), 3);
        public static readonly SoundStyle ToxicWormLaunch = new SoundStyle(PATH + nameof(ToxicWormLaunch), 5);
        public static readonly SoundStyle ToxicWormSpawn = new SoundStyle(PATH + nameof(ToxicWormSpawn), 3);
        public static readonly SoundStyle WispExplosion = new SoundStyle(PATH + nameof(WispExplosion), 4);

        public static SoundStyle WithPitchVariance(this SoundStyle style, float pitchVariance)
        {
            style.PitchVariance = pitchVariance;
            return style;
        }
    }
}
