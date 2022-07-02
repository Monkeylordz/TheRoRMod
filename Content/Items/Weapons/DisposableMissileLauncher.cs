using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Content.Projectiles;
using RoRMod.Utilities;
using Microsoft.Xna.Framework;

namespace RoRMod.Content.Items.Weapons
{
    internal class DisposableMissileLauncher : ModItem
    {
        public const float MissileSpeed = 12f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Continuously launches missiles");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 26;
            Item.scale = 0.8f;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();

            Item.damage = 1;
            Item.crit = 4;
            Item.knockBack = 1;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = RoRSound.MissileFire.WithVolumeScale(0.4f).WithPitchVariance(0.2f);
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<MissileProjectile>();
            Item.shootSpeed = MissileSpeed;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation += new Vector2(-player.direction * Item.width / 2, -Item.height / 2);
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) * 0.9f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Offset X in item direction
            position.X += Item.direction * 16;
            // Offset Y upward
            position.Y -= 16;

            // Shoot directly upward
            velocity = -Vector2.UnitY;
        }
    }
}
