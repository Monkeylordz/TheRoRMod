using Terraria;
using Terraria.ModLoader;
using RoRMod.Utilities;
using Terraria.DataStructures;
using RoRMod.Content.Common;
using ReLogic.Utilities;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class Medkit : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Heal 3 seconds after taking damage");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MedkitPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class MedkitPlayer : AccessoryPlayer
    {
        private CooldownTimer cooldownTimer = new CooldownTimer(CooldownTimer.ResetMode.Manual, 180);
        private SlotId medkitSoundSlotId;

        public override void UpdateEquips()
        {
            // Heal for 5% after cooldown
            if (AccessoryEquipped)
            {
                // Heal imminent
                if (cooldownTimer.OnCooldown)
                {
                    cooldownTimer.Tick();

                    // Play sound with 45 frames left
                    if (cooldownTimer.CurrentTime == 45)
                    {
                        medkitSoundSlotId = SoundEngine.PlaySound(RoRSound.Medkit.WithVolumeScale(0.15f), Player.Center);
                    }

                    // Cooldown just finished
                    if (cooldownTimer.OffCooldown)
                    {
                        PlayerUtils.HealPercentage(Player, 0.05f);
                    }
                }
            }
            else
            {
                cooldownTimer.ResetToZero();
                CancelSound();
            }
        }

        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            // Start heal cooldown
            if (AccessoryEquipped)
            {
                cooldownTimer.ResetToMax();

                CancelSound();
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            cooldownTimer.ResetToZero();
            CancelSound();
        }

        private void CancelSound()
        {
            if (medkitSoundSlotId != SlotId.Invalid && SoundEngine.TryGetActiveSound(medkitSoundSlotId, out ActiveSound sound)) 
            {
                sound.Stop();
                medkitSoundSlotId = SlotId.Invalid;
            }
        }
    }
}
