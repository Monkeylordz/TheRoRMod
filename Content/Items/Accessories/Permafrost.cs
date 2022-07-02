using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class Permafrost : ModItem
    {
        public const int FreezeDuration = 60;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Chance on hit to freeze enemies");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = ModContent.RarityType<Rarities.RareRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PermafrostPlayer>().SetAccessory(Item, hideVisual);
        }
    }

    internal class PermafrostPlayer : OnHitAccessoryPlayer
    {
        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // 5% chance to freeze target
            if (AccessoryEquipped && PlayerUtils.ChanceRoll(Player, 0.05f))
            {
                target.GetGlobalNPC<BuffableNPC>().Buffs.AddBuff(new FreezeBuff(target, Permafrost.FreezeDuration));
            }
        }
    }
}
