using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class FiremansBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireman's Boots");
            Tooltip.SetDefault("Leave a trail of fire");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = Item.rare = ModContent.RarityType<Rarities.RareRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FiremansBootsPlayer>().SetAccessory(Item, hideVisual);
        }
    }

    internal class FiremansBootsPlayer : AccessoryPlayer
    {
        public const float BurnMultiplier = 0.6f;
        public const float FiretrailMoveDistanceTheshold = 30f;
        protected Vector2 firetrailLastPosition = new Vector2(float.NaN);

        public override void UpdateEquips()
        {
            if (AccessoryEquipped)
            {
                CreateFiretrail();
            }
        }

        public void CreateFiretrail()
        {
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                if (firetrailLastPosition.HasNaNs())
                {
                    firetrailLastPosition = Player.position;
                }

                // Create firetrail if the player has moved more than the threshold
                if (Vector2.DistanceSquared(Player.position, firetrailLastPosition) >= FiretrailMoveDistanceTheshold * FiretrailMoveDistanceTheshold)
                {
                    IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                    int projectileType = ModContent.ProjectileType<FiretrailProjectile>();
                    Vector2 offsetX = Vector2.UnitX * Main.rand.NextFloat(-Player.width / 2f, Player.width / 2f);
                    Vector2 offsetY = -Vector2.UnitY * 16;
                    Projectile.NewProjectile(spawnSource, Player.Bottom + offsetX + offsetY, Vector2.Zero, projectileType, 
                        (int)(BurnMultiplier * PlayerUtils.BaseDamage(Player)), 0, Player.whoAmI);
                    firetrailLastPosition = Player.position;
                }
            }
        }
    }
}
