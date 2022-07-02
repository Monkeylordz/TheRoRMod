using Microsoft.Xna.Framework;
using RoRMod.Content.Items.Accessories;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace RoRMod.Utilities
{
    internal static class PlayerUtils
    {
        /// <summary>
        /// Returns true if this code is running on a client
        /// </summary>
        /// <returns></returns>
        public static bool IsClient()
        {
            return Main.netMode != NetmodeID.Server;
        }

        /// <summary>
		/// Returns true if the we are the owner's client
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		public static bool IsOwnerClient(int owner)
        {
            return IsClient() && Main.myPlayer == owner;
        }

        /// <summary>
        /// Returns true if we are the server
        /// </summary>
        /// <returns></returns>
        public static bool IsServer()
        {
            return Main.netMode != NetmodeID.MultiplayerClient;
        }

        /// <summary>
        /// Calculates a Player's base damage based on their stats and boss progression
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static int BaseDamage(Player player, bool addVariance = false)
        {
            // Absolute Base: 11 at 100/20, 35 at 400/200, 40 at 500/200
            int baseDamage = (player.statLifeMax / 20) + (player.statManaMax / 20) + 5;
            if (addVariance)
                baseDamage = Main.DamageVar(baseDamage, player.luck);

            if (NPC.downedMoonlord)
                return baseDamage * 4;
            if (NPC.downedAncientCultist)
                return (int)(baseDamage * 3.5);
            if (NPC.downedGolemBoss)
                return baseDamage * 3;
            if (NPC.downedPlantBoss)
                return (int)(baseDamage * 2.5);
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                return baseDamage * 2; // All mechs
            if (NPC.downedMechBossAny)
                return (int)(baseDamage * 1.75);  // Any mech
            if (Main.hardMode)
                return (int)(baseDamage * 1.5); // WoF
            if (NPC.downedBoss3)
                return (int)(baseDamage * 1.33); // Skeletron
            if (NPC.downedBoss2)
                return (int)(baseDamage * 1.25); // BoC/EoW
            if (NPC.downedBoss1)
                return (int)(baseDamage * 1.125); // EoC

            return baseDamage;
        }

        /// <summary>
        /// Rolls the dice with a given probability of success. Allows rerolling for a more favorable outcome.
        /// </summary>
        /// <param name="player">The player rolling for success.</param>
        /// <param name="chance">The chance of success. Must be between 0 and 1.</param>
        /// <param name="useRerolls">Set true if you want to use rerolls based on the player's items.</param>
        /// <returns></returns>
        public static bool ChanceRoll(Player player, float chance, bool useRerolls = true)
        {
            float rolls = 1;
            if (useRerolls)
            {
                // Clover: Add luck to rolls
                if (player.GetModPlayer<CloverPlayer>().AccessoryEquipped)
                {
                    rolls += player.luck;
                }

                // Snake Eyes: 
                if (player.GetModPlayer<SnakeEyesPlayer>().AddReroll)
                {
                    rolls++;
                }
            }

            bool success = false;
            while (rolls > 0 && !success)
            {
                if (rolls < 1)
                {
                    // Partial roll: only helps some of the time
                    success |= (Main.rand.NextFloat() < chance && Main.rand.NextFloat() < rolls);
                }
                else
                {
                    success |= Main.rand.NextFloat() < chance;
                }
                
                rolls--;
            }
            return success;
        }

        /// <summary>
        /// Heal a player by a flat amount of life
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        public static void HealFlat(Player player, int amount)
        {
            player.statLife += amount;
            player.HealEffect(amount);
        }

        /// <summary>
        /// Heal a player by a percentage of their max life
        /// </summary>
        /// <param name="player"></param>
        /// <param name="percentage"></param>
        public static void HealPercentage(Player player, float percentage)
        {
            int lifeGain = (int)(percentage * player.statLifeMax);
            HealFlat(player, lifeGain);
        }

        /// <summary>
        /// Checks if a player is near a hostile NPC
        /// </summary>
        /// <param name="player"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool IsNearHostileNPC(Player player, float radius)
        {
            List<NPC> npcs = NPCUtils.GetNPCs(NPCConditions.Base.Hostile().WithinRadius(player.Center, radius).NotTargetDummy());
            return npcs.Count > 0;
        }
    }
}
