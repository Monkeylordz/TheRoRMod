using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using System.Collections.Generic;
using RoRMod.Utilities;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class IfritsHorn : ModItem
    {
        public const float Radius = 700f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ifrit's Horn");
            Tooltip.SetDefault("Chance on hit to incinerate all nearby enemies");
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
            player.GetModPlayer<IfritsHornPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class IfritsHornPlayer : OnHitAccessoryPlayer
    {
        public static int DamageProjectileType;

        public override void SetStaticDefaults()
        {
            DamageProjectileType = ModContent.ProjectileType<Projectiles.IfritsHornDamageProjectile>();
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.type != DamageProjectileType && proj.type != LegendarySparkPlayer.DamageProjectileType)
            {
                OnHit(target, damage, knockback, crit);
            }
        }

        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // Ifrit's Horn: 8% chance to incinerate on-screen enemies for 220% damage
            if (AccessoryEquipped && PlayerUtils.ChanceRoll(Player, 0.08f))
            {
                List<NPC> onscreenNPCs = NPCUtils.GetNPCs(NPCConditions.Base.Hostile().WithinRadius(Player.Center, IfritsHorn.Radius));
                ProjectileUtils.DamageNPCs(Player, Player.GetSource_Accessory(Accessory), onscreenNPCs, (int)(2.2f * damage), 0, DamageProjectileType);

                // Sound - Fireblast sound
                SoundEngine.PlaySound(RoRSound.Gasoline.WithVolumeScale(0.4f), Player.Center);
            }
        }
    }
}
