using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class MonsterTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Enemies drop hearts more often");
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
            player.GetModPlayer<MonsterToothPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class MonsterToothPlayer : OnKillAccessoryPlayer
    {
        public override void OnKill(NPC target, int damage, float knockback, bool crit)
        {
            // 40% chance to spawn heart
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.4f))
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
