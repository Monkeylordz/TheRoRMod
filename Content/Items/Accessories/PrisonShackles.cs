using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class PrisonShackles : ModItem
    {
        public const float SlowChance = 0.1f;
        public const float SlowAmount = 0.2f;
        public const int SlowDuration = 60;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Chance on hit to slow enemies");
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
            player.GetModPlayer<PrisonShacklesPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class PrisonShacklesPlayer : OnHitAccessoryPlayer
    {
        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // Chance to slow target by 20% for 1 second
            if (AccessoryEquipped && PlayerUtils.ChanceRoll(Player, PrisonShackles.SlowChance))
            {
                target.GetGlobalNPC<BuffableNPC>().Buffs.AddBuff(new SlowBuff(target, PrisonShackles.SlowDuration, PrisonShackles.SlowAmount));
            }
        }
    }
}
