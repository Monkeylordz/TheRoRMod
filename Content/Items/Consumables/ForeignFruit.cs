using RoRMod.Content.Common;
using RoRMod.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RoRMod.Content.Items.Consumables
{
    internal class ForeignFruit : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Heals 50% of your max life, but has longer potion sickness");
            SacrificeTotal = 30;
        }

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 999;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 0, 25);
            Item.rare = ItemRarityID.Orange;

            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = RoRSound.ForeignFruit.WithVolumeScale(0.5f);
            Item.useTurn = true;

            Item.healLife = 50;
            Item.potion = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");
            if (line != null)
            {
                line.Text = Language.GetTextValue("CommonItemTooltip.RestoresLife", $"{Main.LocalPlayer.statLifeMax2 / 2}");
            }
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            healValue = player.statLifeMax2 / 2;
        }

        public override bool? UseItem(Player player)
        {
            // 90 second potion sickness
            player.AddBuff(BuffID.PotionSickness, 5400);
            return true;
        }
    }
}
