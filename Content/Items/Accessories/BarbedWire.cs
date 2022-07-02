using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RoRMod.Content.Items.Accessories
{
    internal class BarbedWire : ModItem
    {
        public const float Radius = 250f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Enemies close to you take 50% more damage");
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
            player.GetModPlayer<BarbedWirePlayer>().SetAccessory(Item, hideVisual);
        }
    }

    internal class BarbedWirePlayer : ModifyHitAccessoryPlayer
    {
        private UniqueProjectileTracker barbedWire = new UniqueProjectileTracker(ModContent.ProjectileType<Projectiles.BarbedWireProjectile>());

        public override void UpdateEquips()
        {
            if (AccessoryEquipped && AccessoryVisible)
            {
                // Spawn barbed wire
                if (!barbedWire.Alive)
                {
                    IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                    barbedWire.Spawn(Player, spawnSource, Player.Center, Vector2.Zero, 0, 0);
                }
            }
            else
            {
                // Remove barbed wire
                barbedWire.Kill();
            }
        }

        // Barbed Wire: 50% extra damage to nearby enemies
        public override void ModifyHit(NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (AccessoryEquipped)
            {
                float sqrDist = Vector2.DistanceSquared(target.Center, Player.Center);
                if (sqrDist <= BarbedWire.Radius * BarbedWire.Radius)
                {
                    damage = (int)(damage * 1.5f);
                }
            }
        }
    }
}
