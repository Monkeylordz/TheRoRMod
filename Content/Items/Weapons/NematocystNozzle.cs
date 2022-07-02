using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Weapons
{
    internal class NematocystNozzle : ModItem
    {
        public const float NematocystSpeed = 16f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Fires a volley of nematocysts");
            ItemRaritySystem.BossItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.scale = 0.75f;
            Item.value = Item.buyPrice(0, 6);
            Item.rare = ModContent.RarityType<Rarities.BossRarity>();

            Item.damage = 1;
            Item.crit = 7;
            Item.knockBack = 4;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item85;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.NematocystProjectile>();
            Item.shootSpeed = NematocystSpeed;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) * 0.75f + 5;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Add random spread of +-10 degrees
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
        }
    }
}
