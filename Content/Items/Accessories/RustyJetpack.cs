using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class RustyJetpack : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Grants a jump boost and allows gliding");
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
            // Jump Boost
            player.jumpSpeedBoost += 3.2f;
            player.extraFall += 30;

            // Gliding
            player.slowFall = player.controlJump;
        }
    }
}
