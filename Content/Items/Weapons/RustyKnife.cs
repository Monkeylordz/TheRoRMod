using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using System;
using Terraria.Audio;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Weapons
{
    internal class RustyKnife : ModItem
	{
		public const float BleedMultiplier = 0.7f;
		public const int BleedDuration = 120;

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault(
				"As Weapon:\n" +
                "\tBleeds enemies on hit\n" +
                "As Accessory:\n" +
				"\tChance to bleed enemies on hit");
			ItemRaritySystem.CommonItems.Add(Type);
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 52;
			Item.height = 52;
			Item.value = Item.buyPrice(0, 1);
			Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
			Item.accessory = true;

			Item.damage = 1;
			Item.crit = 4;
			Item.knockBack = 6;
			Item.DamageType = DamageClass.Melee;
			Item.useTime = 27;
			Item.useAnimation = 27;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
		{
			damage.Base = PlayerUtils.BaseDamage(player) - 3;
		}

		public override void UseAnimation(Player player)
		{
			// Turn to face mouse position
			player.ChangeDir(Math.Sign(Main.MouseWorld.X - player.Center.X));
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
			BuffableNPC buffableNPC = target.GetGlobalNPC<BuffableNPC>();
			buffableNPC.Buffs.AddBuff(new BleedBuff(buffableNPC, (int)(BleedMultiplier * damage), BleedDuration));

			SoundEngine.PlaySound(RoRSound.DaggerHit.WithVolumeScale(0.25f), target.Center);
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<RustyKnifePlayer>().SetAccessory(Item, hideVisual);
		}
    }


    internal class RustyKnifePlayer : OnHitAccessoryPlayer
    {
        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
			// Rusty Knife: 15% chance to Bleed
			if (AccessoryEquipped)
			{
				if (PlayerUtils.ChanceRoll(Player, 0.15f))
				{
					BuffableNPC buffableNPC = target.GetGlobalNPC<BuffableNPC>();
					buffableNPC.Buffs.AddBuff(new BleedBuff(buffableNPC, (int)(RustyKnife.BleedMultiplier * damage), RustyKnife.BleedDuration));

					SoundEngine.PlaySound(RoRSound.DaggerHit.WithVolumeScale(0.25f), target.Center);
				}
			}
		}
    }
}