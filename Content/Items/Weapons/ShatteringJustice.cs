using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using System;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Weapons
{
    internal class ShatteringJustice : ModItem
    {
        public const int ShatterDuration = 120;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault(
                "As Weapon:\n" +
                "\tShatters enemy defense\n" +
                "As Accessory:\n" +
                "\tChance on hit to shatter enemy defense");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.scale = 1.25f;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = ModContent.RarityType<Rarities.RareRarity>();
            Item.accessory = true;

            Item.damage = 1;
            Item.crit = 5;
            Item.knockBack = 6;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            // Item.hammer = 5;
            // Maybe shoot paladin's hammer-like projectile
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) * 1.15f;
        }

        public override void UseAnimation(Player player)
        {
            // Turn to face mouse position
            player.ChangeDir(Math.Sign(Main.MouseWorld.X - player.Center.X));
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            BuffableNPC buffableNPC = target.GetGlobalNPC<BuffableNPC>();
            if (buffableNPC.Buffs.TryGetBuff(out ShatteredArmorBuff buff))
            {
                buff.Apply(ShatterDuration);
            }
            else
            {
                buffableNPC.Buffs.AddBuff(new ShatteredArmorBuff(buffableNPC, ShatterDuration));
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ShatteringJusticePlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class ShatteringJusticePlayer : OnHitAccessoryPlayer
    {
        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // Shattering Justice: Shatter Armor at 15% chance
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.15f))
                {
                    BuffableNPC buffableNPC = target.GetGlobalNPC<BuffableNPC>();
                    if (buffableNPC.Buffs.TryGetBuff(out ShatteredArmorBuff buff))
                    {
                        buff.Apply(ShatteringJustice.ShatterDuration);
                    }
                    else
                    {
                        buffableNPC.Buffs.AddBuff(new ShatteredArmorBuff(buffableNPC, ShatteringJustice.ShatterDuration));
                    }
                }
            }
        }
    }
}
