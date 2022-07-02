using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using RoRMod.Utilities;
using Terraria.DataStructures;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class HermitsScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hermit's Scarf");
            Tooltip.SetDefault("Gain a 20% chance to evade attacks");
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
            player.GetModPlayer<HermitsScarfPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class HermitsScarfPlayer : AccessoryPlayer
    {
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            // Hermit's Scarf: 20% evade chance
            if (AccessoryEquipped)
            {
                if (PlayerUtils.ChanceRoll(Player, 0.2f, false))
                {
                    playSound = false;
                    genGore = false;

                    // Sound
                    SoundEngine.PlaySound(SoundID.Item1, Player.Center);

                    // Dust
                    for (int i = 0; i < 10; i++)
                        Dust.NewDust(Player.TopLeft, Player.width, Player.height, DustID.Smoke);

                    Player.GiveImmuneTimeForCollisionAttack(30);
                    return false;
                }
            }

            return true;
        }
    }
}
