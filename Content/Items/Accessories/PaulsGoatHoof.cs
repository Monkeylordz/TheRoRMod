using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class PaulsGoatHoof : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Paul's Goat Hoof");
            Tooltip.SetDefault("20% increased movement speed");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 54;
            Item.height = 54; 
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.2f;
            player.runAcceleration += 0.1f;
            player.accRunSpeed += 0.1f;
            player.maxRunSpeed += 0.2f;
        }
    }
}
