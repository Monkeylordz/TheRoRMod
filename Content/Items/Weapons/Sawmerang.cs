using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using RoRMod.Content.Rarities;

namespace RoRMod.Content.Items.Weapons
{
    internal class Sawmerang : ModItem
    {
        public const float Speed = 10f;
        public const int TravelTime = 40;
        public const float ReturnSpeed = 12f;
        public const float ReturnAcceleration = 2.5f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Chance to bleed enemies on hit.");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.scale = 0.75f;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<CommonRarity>();

            Item.damage = 1;
            Item.crit = 5;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.SawmerangProjectile>();
            Item.shootSpeed = Speed;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) * 0.7f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Move up slightly
            position.Y -= 5f;
        }
    }
}
