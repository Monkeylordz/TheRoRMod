using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using Terraria.Audio;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class FireShield : ModItem
    {
        public const float BlastRadius = 150f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Explode when you take damage");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 48;
            Item.height = 48;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
            Item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FireShieldPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class FireShieldPlayer : AccessoryPlayer
    {
        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            // Fire Shield: Explode when hit
            if (AccessoryEquipped)
            {
                if (PlayerUtils.IsOwnerClient(Player.whoAmI))
                {
                    ProjectileUtils.CreateExplosion(Player, Player.GetSource_Accessory(Accessory), Player.Center,
                    FireShield.BlastRadius, PlayerUtils.BaseDamage(Player) * 3, 6f);
                }

                // Play Sound
                SoundEngine.PlaySound(SoundID.Item14.WithVolumeScale(0.75f), Player.Center);
            }
        }
    }
}
