using Terraria;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class TelescopicSight : ModItem
    {
        public const float InstakillChance = 0.02f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Gain a 2% chance to instantly kill any non-boss enemy");
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
            player.GetModPlayer<TelescopicSightPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class TelescopicSightPlayer : ModifyHitAccessoryPlayer
    {
        public override void ModifyHit(NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (AccessoryEquipped)
            {
                // Chance to instakill non-boss
                if (!target.boss && PlayerUtils.ChanceRoll(Player, TelescopicSight.InstakillChance))
                {
                    damage = target.life + target.defense;

                    // TODO: dust

                    SoundEngine.PlaySound(RoRSound.Instakill.WithVolumeScale(0.4f), target.Center);
                }
            }
        }
    }
}
