using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Content.Projectiles;
using RoRMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace RoRMod.Content.Items.Accessories
{
    internal class ChargefieldGenerator : ModItem
    {
        public const float InitialRadius = 100f;
        public const float RadiusIncrease = 50f;
        public const int Duration = 450;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Generate a lightning ring around you whenever you kill an enemy");
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
            player.GetModPlayer<ChargefieldGeneratorPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class ChargefieldGeneratorPlayer : OnKillAccessoryPlayer
    {
        public const int MaxCharge = 8;

        private CooldownTimer killTimer = new CooldownTimer(CooldownTimer.ResetMode.Manual, ChargefieldGenerator.Duration, 0);
        private UniqueProjectileTracker chargefield = new UniqueProjectileTracker(ModContent.ProjectileType<ChargefieldProjectile>());
        private int charge;

        public override void UpdateEquips()
        {
            if (AccessoryEquipped && AccessoryVisible)
            {
                // Cooldown
                killTimer.Tick();
                if (killTimer.OffCooldown)
                {
                    charge = 0;
                }

                // Spawn chargefield when charged for 50% BaseDamage
                if (charge > 0)
                {
                    if (!chargefield.Alive)
                    {
                        chargefield.Spawn(Player, Player.GetSource_Accessory(Accessory), Player.Center, Vector2.Zero,
                            PlayerUtils.BaseDamage(Player) / 2, 0);
                    }

                    (chargefield.Projectile.ModProjectile as ChargefieldProjectile).Charge = charge;
                }
                else
                {
                    chargefield.Kill();
                }
            }
            else
            {
                chargefield.Kill();
                killTimer.ResetToZero();
                charge = 0;
            }
        }

        public override void OnKill(NPC target, int damage, float knockback, bool crit)
        {
            // Add a charge
            if (AccessoryEquipped)
            {
                if (charge < MaxCharge)
                {
                    charge++;
                }
                killTimer.ResetToMax();
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (AccessoryEquipped)
            {
                chargefield.Kill();
                killTimer.ResetToZero();
                charge = 0;
            }
        }
    }
}
