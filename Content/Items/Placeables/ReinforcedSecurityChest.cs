using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Items.Placeables
{
    internal class ReinforcedSecurityChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("UESC Reinforced Security Chest");

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Blue;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 750;
            Item.createTile = ModContent.TileType<Tiles.ReinforcedSecurityChestTile>();
        }
    }
}
