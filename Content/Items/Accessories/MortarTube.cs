using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Projectiles;
using RoRMod.Utilities;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class MortarTube : ModItem
    {
        public const int ExpectedHitTime = 45;
        public const float ShellBlastRadius = 75f;
        public const float Gravity = 0.6f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Chance on hit to launch a barrage of mortar shells");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MortarTubePlayer>().SetAccessory(Item, hideVisual);
        }
    }

    internal class MortarTubePlayer : OnHitAccessoryPlayer
    {
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.type != ModContent.ProjectileType<MortarProjectile>())
                OnHit(target, damage, knockback, crit);
        }

        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // Mortar Tube: 8% chance to launch mortar for 170% damage
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.08f))
                {
                    LaunchMortar(target, damage);
                }
            }
        }

        private void LaunchMortar(NPC target, int damage)
        {
            // Launch mortar
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                int projectileType = ModContent.ProjectileType<MortarProjectile>();
                int count = Main.rand.Next(1, 4);
                for (int i = 0; i < count; i++)
                {
                    // Kinematics to hit target at given time
                    float launchVelocityX = (target.Center.X - Player.Center.X) / MortarTube.ExpectedHitTime;
                    float launchVelocityY = (target.Center.Y - Player.Center.Y) / MortarTube.ExpectedHitTime - 0.5f * MortarTube.Gravity * MortarTube.ExpectedHitTime;
                    Vector2 velocity = new Vector2(launchVelocityX, launchVelocityY);

                    // Add randomness
                    velocity = velocity.RotatedByRandom(0.1);

                    Projectile.NewProjectile(spawnSource, Player.Top, velocity, projectileType, (int)(damage * 1.7f), 2f, Player.whoAmI);
                }
            }

            // Sound
            // Blowpipe-like launch sound
            float rand = Main.rand.NextFloat();
            if (rand < 0.33f)
            {
                SoundEngine.PlaySound(SoundID.Item63, Player.Top);
            }
            else if (rand < 0.66f)
            {
                SoundEngine.PlaySound(SoundID.Item64, Player.Top);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Item65, Player.Top);
            }
        }
    }
}
