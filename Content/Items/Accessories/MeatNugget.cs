using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class MeatNugget : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Small chance on hit for enemies to drop a heart");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 50;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MeatNuggetPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class MeatNuggetPlayer : OnHitAccessoryPlayer
    {
        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // Meat Nugget: 1.5% chance to spawn heart
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.015f))
                {
                    if (PlayerUtils.IsServer())
                    {
                        target.DropItemInstanced(target.Center, target.Hitbox.Size(), ItemID.Heart);
                    }
                }
            }
        }
    }
}
