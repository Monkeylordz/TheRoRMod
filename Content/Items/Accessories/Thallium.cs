using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class Thallium : ModItem
    {
        public const float Chance = 0.025f;
        public const int PoisonDuration = 180;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Small chance for attacks to deal 5 times damage and afflict poison");
            ItemRaritySystem.RareItems.Add(Type);
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
            player.GetModPlayer<ThalliumPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class ThalliumPlayer : ModifyHitAccessoryPlayer
    {
        public override void ModifyHit(NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            // Chance for 500% damage and poison
            if (AccessoryEquipped && PlayerUtils.ChanceRoll(Player, Thallium.Chance))
            {
                damage *= 5;
                BuffableNPC npc = target.GetGlobalNPC<BuffableNPC>();
                npc.Buffs.AddBuff(new PoisonBuff(npc, damage / 2, Thallium.PoisonDuration));
            }
        }
    }
}
