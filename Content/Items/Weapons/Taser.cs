using RoRMod.Content.Buffs;
using RoRMod.Content.Common;
using RoRMod.Content.Rarities;
using RoRMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Items.Weapons
{
    public class Taser : ModItem
    {
        public const int StunDurationWeapon = 15;
        public const int StunDurationAccessory = 30;
        public const float StunChance = 0.05f;
        public const float MaxFlyDistance = 800;
        public const float MaxAttachDistance = 1200;
        public const float ShootSpeed = 22.5f;
        public const float RetractSpeed = 40f;

        private static int projectileType;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault(
                "As Weapon:\n" +
                "\tAttaches to enemies and stuns them\n" +
                "As Accessory:\n" +
                "\tChance on hit to stun enemies\n");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
            projectileType = ModContent.ProjectileType<Projectiles.TaserProjectile>();
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 42;
            Item.scale = 0.75f;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<CommonRarity>();
            Item.accessory = true;

            Item.damage = 1;
            Item.crit = 5;
            Item.knockBack = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.reuseDelay = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item61;
            Item.autoReuse = true;
            Item.shoot = projectileType;
            Item.shootSpeed = ShootSpeed;
            Item.noMelee = true;
            Item.channel = true;
            Item.useTurn = true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) * 0.9f;
        }

        public override bool? CanAutoReuseItem(Player player)
        {
            return player.ownedProjectileCounts[projectileType] <= 0;
        }

        //public override int ChoosePrefix(UnifiedRandom rand)
        //{
        //    // PrefixID.

        //    return base.ChoosePrefix(rand);
        //}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<TaserPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class TaserPlayer : OnHitAccessoryPlayer
    {
        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            if (AccessoryEquipped && PlayerUtils.ChanceRoll(Player, Taser.StunChance))
            {
                target.GetGlobalNPC<BuffableNPC>().Buffs.AddBuff(new StunBuff(target, Taser.StunDurationAccessory));
            }
        }
    }
}
