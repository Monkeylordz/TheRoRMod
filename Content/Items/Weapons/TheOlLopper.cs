using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using System;

namespace RoRMod.Content.Items.Weapons
{
    internal class TheOlLopper : ModItem
    {
        public const float WeaponLifeThresholdPercent = 0.33f;
        public const float AccessoryLifeThresholdPercent = 0.15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Ol' Lopper");
            Tooltip.SetDefault(
                "As Weapon:\n" +
                "\tGuaranteed critical hits against enemies below 33% health\n" +
                "As Accessory:\n" +
                "\tGuaranteed critical hits against enemies below 15% health");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

		public override void SetDefaults()
		{
			Item.width = 58;
			Item.height = 58;
            Item.scale = 1.5f;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = Item.rare = ModContent.RarityType<Rarities.RareRarity>();
            Item.accessory = true;

			Item.damage = 1;
			Item.crit = 6;
			Item.knockBack = 6;
			Item.DamageType = DamageClass.Melee;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
            // Item.axe = 5;
		}

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) * 1.15f + 8;
        }

        public override void UseAnimation(Player player)
        {
            // Turn to face mouse position
            player.ChangeDir(Math.Sign(Main.MouseWorld.X - player.Center.X));
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
			// 100% crit chance against enemies below life threshold
			if ((float)target.life / target.lifeMax <= WeaponLifeThresholdPercent)
            {
				crit = true;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<TheOlLopperPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class TheOlLopperPlayer : ModifyHitAccessoryPlayer
    {
        public override void ModifyHit(NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            // The Ol' Lopper: 100% crit to enemies below life threshold
            if (AccessoryEquipped)
            {
                if ((float)target.life / target.lifeMax <= TheOlLopper.AccessoryLifeThresholdPercent)
                {
                    crit = true;
                }
            }
        }
    }
}
