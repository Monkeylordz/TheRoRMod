using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using Terraria.DataStructures;
using RoRMod.Content.Projectiles;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Weapons
{
    internal class CeremonialDagger : ModItem
    {
        public const float DaggerSpeed = 13f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault(
                "As Weapon:\n" +
                "\tEnemies killed by this dagger burst into more daggers\n" +
                "As Accessory:\n" +
                "\tChance for enemies to burst into daggers when killed");
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
            Item.crit = 6;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<CeremonialDaggerProjectile>();
            Item.shootSpeed = DaggerSpeed;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage.Base = PlayerUtils.BaseDamage(player);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            // Swing style code from vanilla, with rotation adjusted slightly
            float itemShiftX = 24f;
            float itemShiftY = 12f;
            int sign = (player.itemAnimation < player.itemAnimationMax * 0.666f).ToDirectionInt();
            player.itemLocation.X = player.Center.X + sign * (heldItemFrame.Width * 0.5f - itemShiftX) * player.direction;
            player.itemLocation.Y = player.MountedCenter.Y + itemShiftY;
            float rotationOffset = 0.3f + MathHelper.Pi / 6; // default = 0.3f
            player.itemRotation = (((float)player.itemAnimation / player.itemAnimationMax - 0.5f) * 3.5f + rotationOffset) * (-player.direction);
            player.FlipItemLocationAndRotationForGravity();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CeremonialDaggerPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class CeremonialDaggerPlayer : OnKillAccessoryPlayer
    {
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            // Ceremonial Daggers weapon on-kill
            if (proj.type == ModContent.ProjectileType<CeremonialDaggerProjectile>())
            {
                if (target.life <= 0)
                {
                    SpawnDaggers(target, damage, knockback, 4);
                }
            }
            else
            {
                base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
            }
        }

        public override void OnKill(NPC target, int damage, float knockback, bool crit)
        {
            // Ceremonial Dagger: 50% chance of spawning daggers
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.5f))
                {
                    SpawnDaggers(target, damage, knockback, 4);

                }
            }
        }

        private void SpawnDaggers(NPC npc, int damage, float knockback, int amount)
        {
            // Spawn 4 dagger projectiles
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                IEntitySource spawnSource = npc.GetSource_Death();
                int projectileType = ModContent.ProjectileType<CeremonialDaggerProjectile>();
                for (int i = 0; i < amount; i++)
                {
                    Vector2 velocity = -Main.rand.NextVector2Unit(MathHelper.PiOver4, MathHelper.Pi) * CeremonialDagger.DaggerSpeed * 0.75f;
                    Projectile.NewProjectile(spawnSource, npc.Center, velocity, projectileType, damage, knockback, Player.whoAmI);
                }
            }

            SoundEngine.PlaySound(RoRSound.DaggerSpawn.WithVolumeScale(0.5f), npc.Center);
        }
    }
}
