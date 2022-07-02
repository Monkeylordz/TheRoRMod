using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class BoxingGloves : ModItem
    {
        public const float KnockbackIncrease = 5f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases knockback");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 0, 75);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<BoxingGlovesPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class BoxingGlovesPlayer : ModifyHitAccessoryPlayer
    {
        public override void ModifyHit(NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            // Boxing Gloves: Increased knockback
            if (AccessoryEquipped)
            {
                knockback += BoxingGloves.KnockbackIncrease;

                SoundEngine.PlaySound(RoRSound.Punch.WithVolumeScale(0.75f), target.Center);
            }
        }
    }
}
