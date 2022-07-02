using Microsoft.Xna.Framework;
using RoRMod.Content.Projectiles;
using RoRMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class AtGMissileMk1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("AtG Missile Mk. 1");
            Tooltip.SetDefault("Chance on hit to launch a homing missile");
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
            player.GetModPlayer<AtGMk1Player>().SetAccessory(Item, hideVisual);
        }
    }


    internal class AtGMk1Player : OnHitAccessoryPlayer
    {
        // Prevent AtG from proccing itself
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.type != ModContent.ProjectileType<MissileProjectile>())
                OnHit(target, damage, knockback, crit);
        }

        // AtG Mk1: 10% chance on hit to fire missile
        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            if (AccessoryEquipped && PlayerUtils.ChanceRoll(Player, 0.1f))
            {
                FireAtG(damage, -Vector2.UnitY);
            }
        }

        protected void FireAtG(int damage, Vector2 direction)
        {
            // Create an AtG for 300% damage
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                int projectileType = ModContent.ProjectileType<MissileProjectile>();
                Projectile.NewProjectile(spawnSource, Player.Top, direction, projectileType, 3 * damage, 0, Player.whoAmI);
            }

            // Launch Sound
            SoundEngine.PlaySound(RoRSound.MissileFire.WithVolumeScale(0.15f), Player.Top + direction);
        }
    }
}
