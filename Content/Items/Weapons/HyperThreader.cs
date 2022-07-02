using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using Terraria.DataStructures;
using RoRMod.Content.Projectiles;
using Terraria.Audio;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Weapons
{
    internal class HyperThreader : ModItem
    {
        public const float HyperLaserSpeed = 15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hyper-Threader");
            Tooltip.SetDefault(
                "As Weapon:\n" +
                "\tFires a laser that ricochets between enemies\n" +
                "As Accessory:\n" +
                "\tChance to fire a laser whenever you shoot a projectile");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 58;
            Item.scale = 0.75f;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = ModContent.RarityType<Rarities.RareRarity>();
            Item.accessory = true;

            Item.damage = 1;
            Item.crit = 4;
            Item.knockBack = 1;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 9;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item91.WithVolumeScale(0.75f);
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<HyperThreaderProjectile>();
            Item.shootSpeed = HyperLaserSpeed;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) * 0.7f + 4;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<HyperThreaderPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class HyperThreaderPlayer : ShootAccessoryPlayer
    {
        protected override void OnShoot(EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Hyper Threader: 25% chance fire hyper-threader laser on shoot for 40% damage
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.25f))
                {
                    FireLaser(velocity, damage, knockback);
                    SoundEngine.PlaySound(SoundID.Item91.WithVolumeScale(0.75f), Player.Center);
                }
            }
        }

        private void FireLaser(Vector2 velocity, int damage, float knockback)
        {
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                int projectileType = ModContent.ProjectileType<HyperThreaderProjectile>();
                Projectile.NewProjectile(spawnSource, Player.Center, velocity.SafeNormalize(Vector2.Zero) * HyperThreader.HyperLaserSpeed,
                    projectileType, (int)(0.4f * damage), knockback, Player.whoAmI);
            }
        }
    }
}
