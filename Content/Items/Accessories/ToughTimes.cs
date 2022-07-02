using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class ToughTimes : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Greatly increases your defense\n" +
                "Make fun noises when hit (hide visibility to disable)");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 58;
            Item.height = 58;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
            Item.defense = 14;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ToughTimesPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class ToughTimesPlayer : OnGetHitAccessoryPlayer
    {
        public override void OnGetHit(NPC npc, int damage, bool crit)
        {
            if (AccessoryEquipped && AccessoryVisible)
            {
                // Play teddy bear sound when hit (if visible)
                SoundEngine.PlaySound(RoRSound.Bear.WithVolumeScale(0.15f), Player.Center);
            }
        }
    }
}
