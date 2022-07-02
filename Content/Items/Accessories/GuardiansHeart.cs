using Terraria;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using RoRMod.Content.Buffs;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class GuardiansHeart : ModItem
    {
        public const int DefenseBuff = 25;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guardian's Heart");
            Tooltip.SetDefault("Greatly boosts your defense if you don't take damage for 7 seconds");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 50;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GuardiansHeartPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class GuardiansHeartPlayer : IndicatorBuffAccessoryPlayer<GuardiansHeartBuff>
    {
        private CooldownTimer hitTimer = new CooldownTimer(CooldownTimer.ResetMode.Manual, 420);

        protected override GuardiansHeartBuff GetBuff()
        {
            return new GuardiansHeartBuff(Player);
        }

        public override void UpdateEquips()
        {
            hitTimer.Tick();

            if (AccessoryEquipped)
            {
                // Shield Active
                if (hitTimer.OffCooldown)
                {
                    ApplyBuff();
                    Player.statDefense += GuardiansHeart.DefenseBuff;
                }
            }
            else
            {
                ClearBuff();
            }
        }

        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            // Shield just broken
            if (AccessoryEquipped && hitTimer.OffCooldown)
            {
                ClearBuff();
                SoundEngine.PlaySound(RoRSound.ShieldBreak.WithVolumeScale(0.8f), Player.Center);
            }

            hitTimer.ResetToMax();
        }
    }
}
