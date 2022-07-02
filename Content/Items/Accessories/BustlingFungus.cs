using Terraria;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class BustlingFungus : ModItem
    {
        public const int WaitDuration = 120;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Standing still for 2 seconds greatly increases your life regeneration");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 0, 90);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BustlingFungusPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class BustlingFungusPlayer : IndicatorBuffAccessoryPlayer<BustlingFungusBuff>
    {
        private CooldownTimer waitTimer = new CooldownTimer(CooldownTimer.ResetMode.Manual, BustlingFungus.WaitDuration);

        protected override BustlingFungusBuff GetBuff()
        {
            return new BustlingFungusBuff(Player);
        }

        // Bustling Fungus: Increase lifeRegen when if haven't moved for two seconds
        public override void UpdateEquips()
        {
            if (AccessoryEquipped)
            {
                // Reset on equip
                if (JustEquipped)
                {
                    waitTimer.ResetToMax();
                }

                // Detect movement
                if (Player.oldPosition != Player.position)
                {
                    waitTimer.ResetToMax();
                    ClearBuff();
                }
                else
                {
                    waitTimer.Tick();
                }

                // If still for 2 seconds
                if (waitTimer.OffCooldown)
                {
                    ApplyBuff();

                    // Increase life regen
                    Player.lifeRegen += (int)(Player.statLifeMax * 0.05f);
                }
            }
            else
            {
                ClearBuff();
            }
        }
    }
}
