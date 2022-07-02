using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class SmartShopper : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Enemies drop more money");
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
            player.GetModPlayer<SmartShopperPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class SmartShopperPlayer : ModifyHitAccessoryPlayer
    {
        public override void ModifyHit(NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            // Smart Shopper: 25% more money on kill
            if (AccessoryEquipped && damage >= target.life)
            {
                target.value *= 1.25f;
            }
        }
    }
}
