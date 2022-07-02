using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class LensMakersGlasses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lens Maker's Glasses");
            Tooltip.SetDefault("10% increased critical hit chance");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 44;
            Item.height = 44;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += 10;
        }
    }
}
