using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using RoRMod.Content.Projectiles;
using Terraria.DataStructures;
using RoRMod.Utilities;
using Terraria.Audio;
using RoRMod.Content.Common;
using ReLogic.Utilities;

namespace RoRMod.Content.Items.Accessories
{
    internal class FrostRelic : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Generate icicles whenever you kill enemies");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 54;
            Item.height = 54;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = Item.rare = ModContent.RarityType<Rarities.RareRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FrostRelicPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class FrostRelicPlayer : OnKillAccessoryPlayer
    {
        private CooldownTimer soundTimer = new CooldownTimer(CooldownTimer.ResetMode.Automatic, 40, 0);
        private SlotId swirlSoundSlotId = SlotId.Invalid;
        private SlotId windSoundSlotId = SlotId.Invalid;

        public override void UpdateEquips()
        {
            if (AccessoryEquipped)
            {
                // Play sounds if we have icicles
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<FrostRelicProjectile>()] > 0)
                {
                    if (soundTimer.OffCooldown)
                    {
                        swirlSoundSlotId = SoundEngine.PlaySound(RoRSound.IceSwirl.WithVolumeScale(0.2f), Player.Center);
                        windSoundSlotId = SoundEngine.PlaySound(RoRSound.IceWind.WithVolumeScale(0.1f), Player.Center);
                    }
                    soundTimer.Tick();
                }
                else
                {
                    CancelSounds();
                }
            }
            else if (soundTimer.OnCooldown)
            {
                CancelSounds();
            }
        }

        public override void OnKill(NPC target, int damage, float knockback, bool crit)
        {
            // Spawn 2 frost relic icicles for 200% base damage
            if (AccessoryEquipped)
            {
                if (PlayerUtils.IsOwnerClient(Player.whoAmI))
                {
                    for (int i = 0; i < 1 + Main.rand.Next(3); i++)
                    {
                        IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                        int projectileType = ModContent.ProjectileType<FrostRelicProjectile>();
                        Projectile.NewProjectile(spawnSource, Player.Center, Vector2.Zero, projectileType, 2 * PlayerUtils.BaseDamage(Player), 0, Player.whoAmI);
                    }
                }

                // Create icicle sound
                SoundEngine.PlaySound(SoundID.Item30);
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            CancelSounds();
        }

        private void CancelSounds()
        {
            // Stop sounds
            if (swirlSoundSlotId != SlotId.Invalid && SoundEngine.TryGetActiveSound(swirlSoundSlotId, out ActiveSound swirlSound))
            {
                swirlSound.Stop();
                swirlSoundSlotId = SlotId.Invalid;
            }
            if (windSoundSlotId != SlotId.Invalid && SoundEngine.TryGetActiveSound(windSoundSlotId, out ActiveSound windSound))
            {
                windSound.Stop();
                windSoundSlotId = SlotId.Invalid;
            }
            soundTimer.ResetToZero();
        }
    }
}
