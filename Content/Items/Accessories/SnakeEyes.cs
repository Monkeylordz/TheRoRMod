using RoRMod.Content.Buffs;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace RoRMod.Content.Items.Accessories
{
    internal class SnakeEyes : ModItem
    {
        public const int BuffDuration = 180;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Kills temporarily improve your chance-based effects (Risk of Rain effects only)");
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
            player.GetModPlayer<SnakeEyesPlayer>().SetAccessory(Item, hideVisual);
        }
    }

    internal class SnakeEyesPlayer : OnKillAccessoryPlayer
    {
        public bool AddReroll => playerBuffs.HasBuff<SnakeEyesBuff>();

        private BuffManager playerBuffs;

        public override void Initialize()
        {
            playerBuffs = Player.GetModPlayer<BuffablePlayer>().Buffs;
        }

        public override void UpdateEquips()
        {
            if (!AccessoryEquipped && playerBuffs.HasBuff<SnakeEyesBuff>())
            {
                playerBuffs.ClearBuff<SnakeEyesBuff>();
            }
        }

        public override void OnKill(NPC target, int damage, float knockback, bool crit)
        {
            if (AccessoryEquipped)
            {
                playerBuffs.AddBuff(new SnakeEyesBuff(Player, SnakeEyes.BuffDuration));
            }
        }
    }
}
