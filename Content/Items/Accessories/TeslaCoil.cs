using Terraria;
using Terraria.ModLoader;
using RoRMod.Utilities;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class TeslaCoil : ModItem
    {
        public const float DetectRadius = 450f;
        public const float ChainRadius = 450f;
        public const int MaxChain = 15;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Continuously shocks nearby enemies");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = ModContent.RarityType<Rarities.RareRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<TeslaCoilPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class TeslaCoilPlayer : AccessoryPlayer
    {
        private CooldownTimer cooldownTimer = new CooldownTimer(CooldownTimer.ResetMode.Automatic, 60);

        public override void UpdateEquips()
        {
            // Tesla Coil: Zap nearby enemies every second
            if (AccessoryEquipped)
            {
                cooldownTimer.Tick();
                if (cooldownTimer.OffCooldown)
                {
                    TeslaCoilZap();
                }
            }
        }

        private void TeslaCoilZap()
        {
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                // Find nearest NPC
                NPC nearestNPC = NPCUtils.GetClosestNPC(Player.Center, NPCConditions.Base.CanBeChased().WithinRadius(Player.Center, TeslaCoil.DetectRadius));
                if (nearestNPC == null) return;

                // Get enemies near to that NPC
                List<NPC> nearbyNPCs = NPCUtils.GetClosestNPCs(nearestNPC.Center, NPCConditions.Base.CanBeChased().WithinRadius(nearestNPC.Center, TeslaCoil.ChainRadius), TeslaCoil.MaxChain);

                // Deal 100% base damage
                ProjectileUtils.DamageNPCs(Player, Player.GetSource_Accessory(Accessory), nearbyNPCs, PlayerUtils.BaseDamage(Player, true), 0);

                // Create chain lightning, including our location
                var locations = new List<Vector2>();
                locations.Add(Player.Center);
                foreach (NPC npc in nearbyNPCs)
                {
                    locations.Add(npc.Center);
                }
                DustUtils.ChainLightning(locations);

                // Sound
                SoundEngine.PlaySound(RoRSound.ChainLightning.WithVolumeScale(0.25f), Player.Center);
            }
        }
    }
}
