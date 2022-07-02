using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Content.Projectiles;
using RoRMod.Utilities;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace RoRMod.Content.Items.Accessories
{
    internal class ConcussionGrenade : ModItem
    {
        public const float LaunchChance = 0.08f;
        public const float LaunchSpeed = 12f;
        public const float BlastRadius = 250f;
        public const int StunDuration = 60;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Chance on hit to launch a grenade that stuns enemies");
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
            player.GetModPlayer<ConcussionGrenadePlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class ConcussionGrenadePlayer : OnHitAccessoryPlayer
    {
        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // Chance to launch grenade for 50% damage
            if (AccessoryEquipped && PlayerUtils.IsOwnerClient(Player.whoAmI) && PlayerUtils.ChanceRoll(Player, ConcussionGrenade.LaunchChance))
            {
                IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                int projectileType = ModContent.ProjectileType<ConcussionGrenadeProjectile>();
                Vector2 velocity = (target.Center - Player.Center).SafeNormalize(Vector2.Zero) * ConcussionGrenade.LaunchSpeed;
                velocity = velocity.RotatedByRandom(0.1f);
                Projectile.NewProjectile(spawnSource, Player.Center, velocity, projectileType, PlayerUtils.BaseDamage(Player) / 2, 0, Player.whoAmI);
            }
        }
    }
}
