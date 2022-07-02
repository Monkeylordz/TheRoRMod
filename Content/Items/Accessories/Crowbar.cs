using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class Crowbar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Deal +50% damage to enemies above 90% health");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

		public override void SetDefaults()
		{
			Item.accessory = true;
			Item.width = 54;
			Item.height = 54;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrowbarPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class CrowbarPlayer : ModifyHitAccessoryPlayer
    {
        public override void ModifyHit(NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            // Crowbar: Deal +50% damage to enemies above 90% health
            if (AccessoryEquipped)
            {
                if (target.GetLifePercent() >= 0.9f)
                {
                    damage = (int)(damage * 1.5f);

                    SoundEngine.PlaySound(RoRSound.Crowbar.WithVolumeScale(0.2f), target.Center);
                }
            }
        }
    }
}
