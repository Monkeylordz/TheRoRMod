using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using Terraria.DataStructures;
using RoRMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class LaserTurbine : ModItem
    {
        public const int Threshold = 1000;
        public const int damageCharge = 5;
        public const int critDamageCharge = 15;
        public const int killCharge = 75;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Kills and damage charge the laser turbine\n" +
                "Fires twin lasers when fully charged");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = ModContent.RarityType<Rarities.RareRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<LaserTurbinePlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class LaserTurbinePlayer : AccessoryPlayer
    {
        private CooldownTimer laserCooldownTimer = new CooldownTimer(CooldownTimer.ResetMode.Manual, 300);
        private CooldownTimer laserChargeLossTime = new CooldownTimer(CooldownTimer.ResetMode.Automatic, 12);

        private UniqueProjectileTracker device = new UniqueProjectileTracker(ModContent.ProjectileType<LaserTurbineDevice>());
        private int laserTurbineCharge;

        public override void UpdateEquips()
        {
            // Laser Turbine cooldowns
            if (AccessoryEquipped)
            {
                laserCooldownTimer.Tick();

                // Lose 5 charge/sec
                laserChargeLossTime.Tick();
                if (laserChargeLossTime.OffCooldown)
                    laserTurbineCharge--;
            }
            else
            {
                laserTurbineCharge = 0;
                laserCooldownTimer.ResetToZero();
            }

            // Laser Turbine: Keep device above head, fires a laser at full charge
            if (AccessoryEquipped && AccessoryVisible)
            {
                // Spawn Laser Turbine Device if it doesn't exist
                if (!device.Alive)
                {
                    IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                    device.Spawn(Player, spawnSource, Player.Top, Vector2.Zero, 0, 0);
                }

                if (laserCooldownTimer.OffCooldown)
                {
                    // Update Device
                    LaserTurbineDevice turbine = device.Projectile.ModProjectile as LaserTurbineDevice;
                    if (turbine != null)
                    {
                        turbine.UpdateCharge(laserTurbineCharge);

                        // Fire Laser
                        if (laserTurbineCharge > LaserTurbine.Threshold)
                        {
                            // Fire Laser turbine w/ 5s cooldown
                            turbine.FireLasers(5 * PlayerUtils.BaseDamage(Player));
                            laserTurbineCharge -= LaserTurbine.Threshold;
                            turbine.UpdateCharge(laserTurbineCharge);
                            laserCooldownTimer.ResetToMax();
                        }
                    }
                }
            }
            else
            {
                device.Kill();
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            OnHit(crit);
            if (target.life <= 0) OnKill();
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            OnHit(crit);
            if (target.life <= 0) OnKill();
        }

        private void OnHit(bool crit)
        {
            // Charge Laser Turbine on hit
            if (AccessoryEquipped && AccessoryVisible)
            {
                if (!laserCooldownTimer.OnCooldown)
                {
                    if (crit)
                    {
                        laserTurbineCharge += LaserTurbine.critDamageCharge;
                    }
                    else
                    {
                        laserTurbineCharge += LaserTurbine.damageCharge;
                    }
                }
            }
        }

        private void OnKill()
        {
            // Charge Laser Turbine on kill
            if (AccessoryEquipped && AccessoryVisible)
            {
                if (!laserCooldownTimer.OnCooldown)
                {
                    laserTurbineCharge += LaserTurbine.killCharge;
                }
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            // Reset on death
            laserTurbineCharge = 0;
            laserCooldownTimer.ResetToZero();
            device.Kill();
        }
    }
}
