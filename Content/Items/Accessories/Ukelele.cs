using Terraria;
using Terraria.ModLoader;
using RoRMod.Utilities;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class Ukelele : ModItem
    {
        public const float DetectionRadius = 400f;
        public const int MaxChain = 5;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("20% chance on hit to chain lightning to other nearby enemies");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 54;
            Item.height = 54;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<UkelelePlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class UkelelePlayer : OnHitAccessoryPlayer
    {
        public static int DamageProjectileType;

        public override void SetStaticDefaults()
        {
            DamageProjectileType = ModContent.ProjectileType<Projectiles.UkeleleDamageProjectile>();
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.type != DamageProjectileType)
            {
                base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
            }
        }

        public override void OnHit(NPC target, int damage, float knockback, bool crit)
        {
            // Uke: 20% chance to chain lightning
            if (AccessoryEquipped && Main.rand.NextFloat() <= 0.2f)
            {
                UkeleleChainLightning(target, damage);
            }
        }

        private void UkeleleChainLightning(NPC npc, int damage)
        {
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                // Find nearby NPCs
                NPCConditions conditions = NPCConditions.Base
                    .CanBeChased()
                    .WithinRadius(npc.Center, Ukelele.DetectionRadius)
                    .Exclude(new int[] { npc.whoAmI });
                List<NPC> nearbyNPCs = NPCUtils.GetClosestNPCs(npc.Center, conditions, Ukelele.MaxChain);
                if (nearbyNPCs.Count <= 0) return;

                // Deal 80% damage
                ProjectileUtils.DamageNPCs(Player, Player.GetSource_Accessory(Accessory), nearbyNPCs, (int)(0.8f * damage), 0, DamageProjectileType);

                // Create chain lightning
                List<Vector2> npcLocations = new List<Vector2>();
                npcLocations.Add(npc.Center);
                foreach (NPC target in nearbyNPCs)
                {
                    npcLocations.Add(target.Center);
                }
                DustUtils.ChainLightning(npcLocations);

                // Sound
                SoundEngine.PlaySound(RoRSound.ChainLightning.WithVolumeScale(0.25f), npc.Center);
            }
        }
    }
}
