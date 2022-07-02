using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class EnergyCell : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Boosts your attack and movement speed the less life you have");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 50;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EnergyCellPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class EnergyCellPlayer : AccessoryPlayer
    {
        public override void UpdateEquips()
        {
            // Increases attack speed and movement speed per 1% HP lost
            if (AccessoryEquipped)
            {
                float lifePercentage = (float)Player.statLife / Player.statLifeMax;
                float boost = 1f - lifePercentage;
                Player.GetAttackSpeed(DamageClass.Generic) += boost * 0.33f;
                Player.moveSpeed += boost;
                Player.maxRunSpeed += boost * 0.33f;
            }
        }
    }
}
