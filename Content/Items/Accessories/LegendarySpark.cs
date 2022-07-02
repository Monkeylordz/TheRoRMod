using Terraria;
using Terraria.ModLoader;
using RoRMod.Utilities;
using System.Collections.Generic;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class LegendarySpark : ModItem
    {
        public const float Radius = 800f;   

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Chance on hit to smite all nearby enemies");
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
            player.GetModPlayer<LegendarySparkPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class LegendarySparkPlayer : OnHitAccessoryPlayer
    {
        public static int DamageProjectileType;

        public override void SetStaticDefaults()
        {
            DamageProjectileType = ModContent.ProjectileType<Projectiles.LegendarySparkDamageProjectile>();
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.type != DamageProjectileType && proj.type != IfritsHornPlayer.DamageProjectileType)
            {
                OnHit(target, damage, knockback, crit);
            }
        }

        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // Legendary Spark: 8% chance to smite on-screen enemies for 200% damage
            if (AccessoryEquipped && PlayerUtils.ChanceRoll(Player, 0.08f))
            {
                List<NPC> onscreenNPCs = NPCUtils.GetNPCs(NPCConditions.Base.Hostile().WithinRadius(Player.Center, LegendarySpark.Radius));
                ProjectileUtils.DamageNPCs(Player, Player.GetSource_Accessory(Accessory), onscreenNPCs, 2 * damage, 0, DamageProjectileType);

                SoundEngine.PlaySound(RoRSound.Lightning.WithVolumeScale(0.15f), Player.Center);
            }
        }
    }
}
