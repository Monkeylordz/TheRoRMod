using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Content.Projectiles;
using RoRMod.Utilities;
using Microsoft.Xna.Framework;

namespace RoRMod.Content.Items.Accessories
{
    internal class StickyBomb : ModItem
    {
        public const float BlastRadius = 150f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("8% chance on hit to attach a sticky bomb that detonates after 3 seconds");
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
            player.GetModPlayer<StickyBombPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class StickyBombPlayer : OnHitAtPositionAccessoryPlayer
    {
        public static int ProjectileType;

        public override void SetStaticDefaults()
        {
            ProjectileType = ModContent.ProjectileType<StickyBombProjectile>();
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            // prevent self-proc
            if (proj.type != ProjectileType)
            {
                base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
            }
        }

        public override void OnHit(NPC target, Vector2 hitPosition, int damage, float knockback, bool crit)
        {
            if (AccessoryEquipped)
            {
                // 8% chance to create sticky bomb
                if (PlayerUtils.IsOwnerClient(Player.whoAmI) && PlayerUtils.ChanceRoll(Player, 0.08f))
                {
                    Vector2 offset = (target.Center - hitPosition) * Main.rand.NextFloat(0.5f, 1);
                    Projectile.NewProjectile(Player.GetSource_Accessory(Accessory), target.Center - offset, offset,
                        ProjectileType, (int)(damage * 1.4f), 0, Player.whoAmI, ai1: target.whoAmI);
                }
            }
        }
    }
}
