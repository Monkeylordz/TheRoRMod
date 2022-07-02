using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class ColossalKnurl : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increased max life, health regeneration and defense");
            ItemRaritySystem.BossItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 56;
            Item.height = 56;
            Item.value = Item.buyPrice(0, 3);
            Item.rare = ModContent.RarityType<Rarities.BossRarity>();
            Item.defense = 4;
            Item.lifeRegen = 4;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 40;
        }
    }
}
