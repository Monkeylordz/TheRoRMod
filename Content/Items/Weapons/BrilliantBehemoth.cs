using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using RoRMod.Content.Projectiles;

namespace RoRMod.Content.Items.Weapons
{
    internal class BrilliantBehemoth : ModItem
    {
        public const float BaseShootSpeed = 13f;
        public const float ShootSpeedIncreaseMultiplier = 0.15f;
        public const float BaseBlastRadius = 100f;
        public const float BlastRadiusIncreaseMultiplier = 2f;
        public const int MaxCharge = 90;
        public const int ClusterThresholdCharge = 60;
        public const int ClusterDelay = 10;
        public static float DamageIncreaseMultiplier => Main.hardMode ? 1f : 0.5f;

        private static int BaseProjectileType;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault(
                "As Weapon:\n" +
                "\tCharge up shots for larger blasts\n" +
                "As Accessory:\n" +
                "\tAll your attacks have a chance to explode on hit");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
            BaseProjectileType = ModContent.ProjectileType<BrilliantBehemothBaseProjectile>();
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 54;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = ModContent.RarityType<Rarities.RareRarity>();
            Item.accessory = true;

            Item.damage = 1;
            Item.crit = 3;
            Item.knockBack = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.reuseDelay = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = true;
            Item.noMelee = true;
            Item.shoot = BaseProjectileType;
            Item.shootSpeed = 1;
            Item.channel = true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) * 1.25f;
        }

        public override bool? CanAutoReuseItem(Player player)
        {
            return player.ownedProjectileCounts[BaseProjectileType] <= 0;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BrilliantBehemothPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class BrilliantBehemothPlayer : OnHitAtPositionAccessoryPlayer
    {
        public static int ExplosionProjectileType;

        public override void SetStaticDefaults()
        {
            ExplosionProjectileType = ModContent.ProjectileType<BehemothExplosionProjectile>();
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            // Prevent self-proc
            if (proj.type != ExplosionProjectileType)
            {
                base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
            }
        }

        public override void OnHit(NPC target, Vector2 hitPosition, int damage, float knockback, bool crit)
        {
            // Brilliant Behemoth: 50% chance attack explodes
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.5f))
                {
                    BehemothExplosion(hitPosition, damage);
                }
            }
        }

        private void BehemothExplosion(Vector2 position, int damage)
        {
            // Create an explosion for 40% damage
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                ProjectileUtils.CreateExplosion(Player, Player.GetSource_Accessory(Accessory), position, 
                    BrilliantBehemoth.BaseBlastRadius, (int)(0.4f * damage), 0f, projectileType: ExplosionProjectileType);
            }
        }
    }
}
