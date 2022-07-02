using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class BurningWitness : ModItem
    {
        public const int BuffDuration = 180;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases your movement speed and damage\n" +
                "Gain a temporary trail of fire on kill");
            ItemRaritySystem.BossItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 6);
            Item.rare = ModContent.RarityType<Rarities.BossRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // +10% move speed
            player.moveSpeed += 0.1f;
            player.maxRunSpeed += 0.1f;

            // +5% damage
            player.GetDamage(DamageClass.Generic) *= 1.05f;

            player.GetModPlayer<BurningWitnessPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class BurningWitnessPlayer : FiremansBootsPlayer
    {
        private BuffManager playerBuffs;

        public override void Initialize()
        {
            playerBuffs = Player.GetModPlayer<BuffablePlayer>().Buffs;
        }

        public override void UpdateEquips()
        {
            if (playerBuffs.HasBuff<BurningWitnessBuff>())
            {
                if (AccessoryEquipped)
                {
                    CreateFiretrail();
                }
                else
                {
                    playerBuffs.ClearBuff<BurningWitnessBuff>();
                }
            }
            

        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0) OnKill();
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0) OnKill();
        }

        public void OnKill()
        {
            if (AccessoryEquipped)
            {
                playerBuffs.AddBuff(new BurningWitnessBuff(Player, BurningWitness.BuffDuration));
            }
        }
    }
}
