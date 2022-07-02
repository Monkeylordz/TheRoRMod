using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class BitterRoot : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("+40 max life");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 56;
            Item.height = 56;
            Item.value = Item.buyPrice(0, 0, 75);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 40;
        }
    }
}
