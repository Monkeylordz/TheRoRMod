using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using Microsoft.Xna.Framework;

namespace RoRMod.Content.Items.Accessories
{
    internal class AtGMissileMk2 : AtGMissileMk1
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("AtG Missile Mk. 2");
            Tooltip.SetDefault("Chance on hit to launch three homing missiles");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<Rarities.RareRarity>();
            Item.value = Item.buyPrice(0, 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AtGMk2Player>().SetAccessory(Item, hideVisual);
        }
    }

    internal class AtGMk2Player : AtGMk1Player
    {
        // AtG Mk2: 7% chance on hit to fire 3 missiles
        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            if (AccessoryEquipped && PlayerUtils.ChanceRoll(Player, 0.07f))
            {
                FireAtG(damage, new Vector2(-1, -2));
                FireAtG(damage, new Vector2(0, -1));
                FireAtG(damage, new Vector2(1, -2));
            }
        }
    }
}
