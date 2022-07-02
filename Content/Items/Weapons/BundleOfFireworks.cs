using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Utilities;

namespace RoRMod.Content.Items.Weapons
{
    internal class BundleOfFireworks : ModItem
    {
        public const float FireworkSpeed = 20f;
        public const float BlastRadius = 60f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bundle of Fireworks");
            Tooltip.SetDefault("Launches a flurry of fireworks.");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.scale = 0.8f;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();

            Item.damage = 1;
            Item.crit = 2;
            Item.knockBack = 1;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = RoRSound.FireworkLaunch.WithVolumeScale(0.25f).WithPitchVariance(0.5f);
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.FireworkProjectile>();
            Item.shootSpeed = FireworkSpeed;

            Item.staff[Item.type] = true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) * 0.7f;
        }
    }
}
