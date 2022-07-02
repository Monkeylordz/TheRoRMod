using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class MysteriousVial : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases your life regeneration");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 46;
            Item.height = 46;
            Item.value = Item.buyPrice(0, 0, 70);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
            Item.lifeRegen = 5;
        }
    }
}
