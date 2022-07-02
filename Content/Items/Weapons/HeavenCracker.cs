using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using Terraria.DataStructures;
using RoRMod.Content.Projectiles;
using RoRMod.Content.Common;
using System;

namespace RoRMod.Content.Items.Weapons
{
    internal class HeavenCracker : ModItem
    {
        public const int FissureInterval = 60;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault(
                "As Weapon:\n" +
                "\tAttacks create a dimensional fissure\n" +
                "As Accessory:\n" +
                "\tEvery fourth projectile you shoot pierces infinitely\n" +
                "This drill will pierce the Heavens!\n");
            ItemID.Sets.IsDrill[Type] = true;
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Drax);
            Item.width = 48;
            Item.height = 48;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = ModContent.RarityType<Rarities.RareRarity>();
            Item.accessory = true;

            Item.damage = 1;
            Item.crit = 6;
            Item.knockBack = 5;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.Item22.WithVolumeScale(0.25f);
            Item.shoot = ModContent.ProjectileType<HeavenCrackerProjectile>();
            Item.axe = 0;
            Item.pick = 0;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) * 0.85f;
        }

        public override void UseAnimation(Player player)
        {
            // Turn to face mouse position
            player.ChangeDir(Math.Sign(Main.MouseWorld.X - player.Center.X));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<HeavenCrackerPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class HeavenCrackerPlayer : ShootAccessoryPlayer
    {
        private CooldownTimer heavenCrackerCounter = new CooldownTimer(CooldownTimer.ResetMode.Automatic, 3);

        private IEntitySource currentSource;
        private int currentType;

        public override void ResetEffects()
        {
            base.ResetEffects();
            currentSource = null;
            currentType = -1;
        }

        protected override void OnShoot(EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (AccessoryEquipped)
            {
                heavenCrackerCounter.Tick();
                if (heavenCrackerCounter.OffCooldown)
                {
                    currentSource = source;
                    currentType = type;
                }
            }
        }

        public bool CheckHeavenCrackerMatch(IEntitySource source, int projectileType)
        {
            if (currentType >= 0 && currentSource != null)
            {
                if (projectileType == currentType && currentSource.Equals(source))
                {
                    return true;
                }
            }
            return false;
        }
    }


    internal class HeavenCrackerGlobalProjectile : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // Heaven Cracker active - add pierce
            if (!projectile.npcProj)
            {
                HeavenCrackerPlayer player = Main.player[projectile.owner].GetModPlayer<HeavenCrackerPlayer>();
                if (player.AccessoryEquipped && player.CheckHeavenCrackerMatch(source, projectile.type))
                {
                    projectile.maxPenetrate = -1;
                    projectile.penetrate = -1;
                }
            }
        }
    }
}
