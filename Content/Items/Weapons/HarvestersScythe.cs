using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using System;
using Terraria.Audio;

namespace RoRMod.Content.Items.Weapons
{
    internal class HarvestersScythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvester's Scythe");
            Tooltip.SetDefault(
                "As Weapon:\n" +
                "\tCritical hits heal you\n" +
                "As Accessory:\n" +
                "\t5% increased critical hit chance\n" +
                "\tCritical hits have a chance to heal you");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 54;
            Item.scale = 1.25f;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
            Item.accessory = true;

            Item.damage = 1;
            Item.crit = 16;
            Item.knockBack = 3;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player) + 5;
        }

        public override void UseAnimation(Player player)
        {
            // Turn to face mouse position
            player.ChangeDir(Math.Sign(Main.MouseWorld.X - player.Center.X));
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            // Heal for 3 life on crit
            if (crit)
            {
                PlayerUtils.HealFlat(player, 3);
                SoundEngine.PlaySound(RoRSound.CritHeal.WithVolumeScale(0.4f), player.Center);
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += 5;
            player.GetModPlayer<HavestersScythePlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class HavestersScythePlayer : OnHitAccessoryPlayer
    {
        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // Harvester's: 50% chance to heal 1 life on crit
            if (AccessoryEquipped && crit)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.5f))
                {
                    PlayerUtils.HealFlat(Player, 1);
                    SoundEngine.PlaySound(RoRSound.CritHeal.WithVolumeScale(0.4f), Player.Center);
                }
            }
        }
    }
}
