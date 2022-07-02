using Terraria;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using RoRMod.Content.Buffs;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class SproutingEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases your life regeneration if you haven't taken damage for 7 seconds");
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
            player.GetModPlayer<SproutingEggPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class SproutingEggPlayer : IndicatorBuffAccessoryPlayer<SproutingEggBuff>
    {
        private CooldownTimer hitTimer = new CooldownTimer(CooldownTimer.ResetMode.Manual, 420);

        protected override SproutingEggBuff GetBuff()
        {
            return new SproutingEggBuff(Player);
        }

        public override void UpdateEquips()
        {
            hitTimer.Tick();

            // Sprouting Egg: Increased life regen when not hit
            if (AccessoryEquipped)
            {
                if (hitTimer.OffCooldown)
                {
                    ApplyBuff();
                    Player.lifeRegen += 6;
                }
            }
            else
            {
                ClearBuff();
            }
        }

        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            // Egg was active
            if (AccessoryEquipped && hitTimer.OffCooldown)
            {
                ClearBuff();
                SoundEngine.PlaySound(RoRSound.EggStop.WithVolumeScale(0.5f), Player.Center);
            }

            hitTimer.ResetToMax();
        }
    }
}
