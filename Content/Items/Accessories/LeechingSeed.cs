using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class LeechingSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Melee strikes have a 33% chance to heal you");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 42;
            Item.height = 42;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<LeechingSeedPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class LeechingSeedPlayer : AccessoryPlayer
    {
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            // 33% chance melee strikes heal for 1 life
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.33f))
                {
                    PlayerUtils.HealFlat(Player, 1);
                    SoundEngine.PlaySound(RoRSound.CritHeal.WithVolumeScale(0.3f), Player.Center);
                }
            }
        }
    }
}
