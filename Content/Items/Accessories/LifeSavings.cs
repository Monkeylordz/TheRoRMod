using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using Terraria.DataStructures;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class LifeSavings : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Generates money over time");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<LifeSavingsPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class LifeSavingsPlayer : AccessoryPlayer
    {
        private CooldownTimer cooldownTimer = new CooldownTimer(CooldownTimer.ResetMode.Automatic, 900);

        public override void UpdateEquips()
        {
            if (AccessoryEquipped)
            {
                cooldownTimer.Tick();

                if (cooldownTimer.OffCooldown)
                {
                    IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                    Player.QuickSpawnItem(spawnSource, ItemID.SilverCoin, stack: 5);
                }
            }
            else
            {
                cooldownTimer.ResetToMax();
            }
        }
    }
}
