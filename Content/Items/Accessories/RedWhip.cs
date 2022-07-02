using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class RedWhip : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increases your movement speed greatly when out of combat");
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
            player.GetModPlayer<RedWhipPlayer>().SetAccessory(Item, hideVisual);
        }
    }

    internal class RedWhipPlayer : IndicatorBuffAccessoryPlayer<RedWhipBuff>
    {
        private CooldownTimer hitCooldownTimer = new CooldownTimer(CooldownTimer.ResetMode.Manual, 180);

        protected override RedWhipBuff GetBuff()
        {
            return new RedWhipBuff(Player);
        }

        public override void UpdateEquips()
        {
            hitCooldownTimer.Tick();

            // Red Whip: +40% move speed
            if (AccessoryEquipped)
            {
                if (hitCooldownTimer.OffCooldown)
                {
                    ApplyBuff();
                    Player.moveSpeed += 0.4f;
                    Player.accRunSpeed += 0.2f;
                    Player.maxRunSpeed += 0.2f;
                }
            }
            else
            {
                ClearBuff();
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            OnHit();
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            OnHit();
        }

        public void OnHit()
        {
            // Reset timer when you deal damage
            hitCooldownTimer.ResetToMax();
            ClearBuff();
        }
    }
}
