using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class PredatoryInstincts : ModItem
    {
        public const int MaxStacks = 3;
        public const int Duration = 180;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("5% increased critical hit chance\n" +
                "Critical hits temporarily increase your attack speed");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 60;
            Item.height = 60;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += 5;
            player.GetModPlayer<PredatoryInstinctsPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class PredatoryInstinctsPlayer : OnHitAccessoryPlayer
    {
        private BuffManager playerBuffs;

        public override void Initialize()
        {
            playerBuffs = Player.GetModPlayer<BuffablePlayer>().Buffs;
        }

        public override void UpdateEquips()
        {
            // Predatory: +10% attack speed per stack
            if (playerBuffs.TryGetBuff(out PredatoryInstinctsBuff buff))
            {
                if (AccessoryEquipped)
                {
                    Player.GetAttackSpeed(DamageClass.Generic) += 0.1f * buff.Stacks;
                }
                else
                {
                    playerBuffs.ClearBuff<PredatoryInstinctsBuff>();
                }
            }
        }

        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // Predatory: Apply stack on crit
            if (AccessoryEquipped && crit)
            {
                if(playerBuffs.TryGetBuff(out PredatoryInstinctsBuff buff))
                {
                    // Add additional stack
                    buff.Apply(PredatoryInstincts.Duration);
                }
                else
                {
                    // Add initial stack
                    playerBuffs.AddBuff(new PredatoryInstinctsBuff(Player, PredatoryInstincts.Duration));
                }
            }
        }
    }
}
