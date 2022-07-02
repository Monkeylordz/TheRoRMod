using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using Terraria.DataStructures;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class FilialImprinting : ModItem
    {
        public const int BuffDuration = 600;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Gain one of an attack speed, move speed or life regen buff\n" +
                "The buff type switches every 10 seconds");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FilialImprintingPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class FilialImprintingPlayer : AccessoryPlayer
    {
        private CooldownTimer buffTimer = new CooldownTimer(CooldownTimer.ResetMode.Automatic, FilialImprinting.BuffDuration, 0);
        private FilialBuff currentBuff;
        private FilialBuff[] filialBuffs;

        public override void Initialize()
        {
            filialBuffs = new FilialBuff[]
            {
                new AttackSpeedBuff(Player),
                new MoveSpeedBuff(Player),
                new RegenBuff(Player)
            };
        }

        public override void UpdateEquips()
        {
            if (AccessoryEquipped)
            {
                if (buffTimer.OffCooldown || JustEquipped || currentBuff == null)
                {
                    // Get new buff
                    currentBuff = filialBuffs[Main.rand.Next(filialBuffs.Length)];
                    currentBuff.SetBuff();
                }

                if (buffTimer.OnCooldown)
                {
                    currentBuff.ApplyEffect();
                }

                buffTimer.Tick();
            }
            else
            {
                buffTimer.ResetToZero();
                if (currentBuff != null)
                {
                    currentBuff.ClearBuff();
                }
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (AccessoryEquipped)
            {
                buffTimer.ResetToZero();
            }
        }
    }


    abstract class FilialBuff
    {
        protected Player player;
        protected BuffManager playerBuffs;

        public FilialBuff(Player player)
        {
            this.player = player;
            playerBuffs = player.GetModPlayer<BuffablePlayer>().Buffs;
        }

        public abstract void SetBuff();
        public abstract void ClearBuff();
        public abstract void ApplyEffect();
    }

    class AttackSpeedBuff : FilialBuff
    {
        public AttackSpeedBuff(Player player) : base(player) { }
        public override void SetBuff() => playerBuffs.AddBuff(new FilialAttackSpeedBuff(player));
        public override void ClearBuff() => playerBuffs.ClearBuff<FilialAttackSpeedBuff>();
        public override void ApplyEffect() => player.GetAttackSpeed(DamageClass.Generic) += 0.15f;
    }

    class MoveSpeedBuff : FilialBuff
    {
        public MoveSpeedBuff(Player player) : base(player) { }
        public override void SetBuff() => playerBuffs.AddBuff(new FilialMoveSpeedBuff(player));
        public override void ClearBuff() => playerBuffs.ClearBuff<FilialMoveSpeedBuff>();
        public override void ApplyEffect()
        {
            player.moveSpeed += 0.2f;
            player.runAcceleration += 0.2f;
            player.maxRunSpeed += 0.2f;
        }

    }

    class RegenBuff : FilialBuff
    {
        public RegenBuff(Player player) : base(player) { }
        public override void SetBuff() => playerBuffs.AddBuff(new FilialRegenBuff(player));
        public override void ClearBuff() => playerBuffs.ClearBuff<FilialRegenBuff>();
        public override void ApplyEffect() => player.lifeRegen += 6;
    }
}
