using Terraria;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class WillOTheWisp : ModItem
    {
        public const float BlastRadius = 250f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Will-o'-the-Wisp");
            Tooltip.SetDefault("Enemies you kill have a chance of exploding");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 52;
            Item.height = 52; 
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<WillOTheWispPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class WillOTheWispPlayer : OnKillAccessoryPlayer
    {
        public override void OnKill(NPC target, int damage, float knockback, bool crit)
        {
            // Will-o'-the-Wisp: 50% chance explode on kill
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.5f))
                {
                    WispExplosion(target);
                }
            }
        }

        private void WispExplosion(NPC npc)
        {
            // Explode for 500% base damage
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                ProjectileUtils.CreateExplosion(Player, Player.GetSource_Accessory(Accessory), npc.Center, WillOTheWisp.BlastRadius, 5 * PlayerUtils.BaseDamage(Player, true), 3f);
            }

            SoundEngine.PlaySound(RoRSound.WispExplosion.WithVolumeScale(0.5f), npc.Center);
        }
    }
}
