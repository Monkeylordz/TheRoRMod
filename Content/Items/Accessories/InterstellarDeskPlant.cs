using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using RoRMod.Content.Projectiles;
using RoRMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Items.Accessories
{
    internal class InterstellarDeskPlant : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Gain a chance on kill to spawn a healing plant");
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
            player.GetModPlayer<IDPPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class IDPPlayer : OnKillAccessoryPlayer
    {
        public override void OnKill(NPC target, int damage, float knockback, bool crit)
        {
            // Interstellar Desk Plant: 33% chance to spawn an Alien Plant
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.33f))
                {
                    IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                    int projectileType = ModContent.ProjectileType<InterstellarDeskPlantProjectile>();
                    Projectile.NewProjectile(spawnSource, target.Center, Vector2.Zero, projectileType, 0, 0, Player.whoAmI);
                }
            }
        }
    }
}
