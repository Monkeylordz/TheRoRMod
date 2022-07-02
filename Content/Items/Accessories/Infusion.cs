using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class Infusion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Killing enemies increases your max life");
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
            player.GetModPlayer<InfusionPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class InfusionPlayer : OnKillAccessoryPlayer
    {
        private int infusionCount;

        public override void UpdateEquips()
        {
            // Infusion: Increase max life
            if (AccessoryEquipped)
            {
                Player.statLifeMax2 += infusionCount;
            }
            else
            {
                infusionCount = 0;
            }
        }

        public override void OnKill(NPC target, int damage, float knockback, bool crit)
        {
            // Infusion: +1 Max HP, up to 100
            if (AccessoryEquipped)
            {
                if (infusionCount < 100)
                {
                    infusionCount++;

                    Dust.NewDustPerfect(Player.Top, DustID.HealingPlus, Velocity: new Vector2(0, 3f), Scale: 2f);

                    SoundEngine.PlaySound(RoRSound.Infusion.WithVolumeScale(0.1f), Player.Center);
                }
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            infusionCount = 0;
        }
    }
}
