using RoRMod.Content.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Items.Accessories
{
    internal class FiftySixLeafClover : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("56 Leaf Clover");
            Tooltip.SetDefault("Your luck affects chance-based effects (Risk of Rain effects only)");
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
            player.GetModPlayer<CloverPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class CloverPlayer : AccessoryPlayer
    {
        
    }
}
