using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace RoRMod.Utilities
{
    internal static class NPCUtils
    {
        /// <summary>
        /// Get a list of NPCs based on given conditions.
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static List<NPC> GetNPCs(NPCConditions conditions)
        {
            if (conditions == null) conditions = NPCConditions.Base;
            List<NPC> npcs = new List<NPC>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (conditions.Met(npc))
                    npcs.Add(npc);
            }
            return npcs;
        }

        /// <summary>
        /// Gets the closest NPC to a point based on given conditions.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static NPC GetClosestNPC(Vector2 center, NPCConditions conditions)
        {
            NPC closestNPC = null;
            List<NPC> npcs = GetNPCs(conditions);
            float maxDistanceSqr = float.MaxValue;
            foreach (NPC npc in npcs)
            {
                float distanceSqr = Vector2.DistanceSquared(center, npc.Center);
                if (distanceSqr < maxDistanceSqr)
                {
                    closestNPC = npc;
                    maxDistanceSqr = distanceSqr;
                }
            }

            return closestNPC;
        }

        /// <summary>
        /// Gets a list of closest NPCs to a point based on given conditions.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static List<NPC> GetClosestNPCs(Vector2 center, NPCConditions conditions, int numNPCs)
        {
            List<NPC> npcs = GetNPCs(conditions);
            if (npcs.Count <= 0) return npcs;
            npcs.Sort(new Comparison<NPC>((NPC A, NPC B) => MathF.Sign(Vector2.DistanceSquared(center, A.Center) - Vector2.DistanceSquared(center, B.Center))));
            return npcs.GetRange(0, Math.Min(npcs.Count, numNPCs));
        }
    }

    internal class NPCConditions
    {
        /// <summary>
        /// The baseline NPCCondition with no conditions.
        /// </summary>
        public static NPCConditions Base => new NPCConditions();

        public delegate bool NPCCondition(NPC npc);
        private List<NPCCondition> conditions;

        private NPCConditions() 
        { 
            conditions = new List<NPCCondition>();

            // Non-nullable check
            conditions.Add((NPC npc) => npc != null);

            // Active check
            conditions.Add((NPC npc) => npc.active);
        }

        /// <summary>
        /// Checks if all conditions have been met for an NPC
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public bool Met(NPC npc)
        {
            foreach (NPCCondition condition in conditions)
            {
                if (!condition(npc)) return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a condition
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public NPCConditions WithCondition(NPCCondition condition)
        {
            conditions.Add(condition);
            return this;
        }

        /// <summary>
        /// Ensures the NPC is hostile
        /// </summary>
        /// <returns></returns>
        public NPCConditions Hostile()
        {
            return WithCondition((NPC npc) => !npc.friendly);
        }

        /// <summary>
        /// Ensures the NPC can be chased
        /// </summary>
        /// <returns></returns>
        public NPCConditions CanBeChased()
        {
            return WithCondition((NPC npc) => npc.CanBeChasedBy());
        }

        /// <summary>
        /// Ensures the NPC is within some radius around a point.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public NPCConditions WithinRadius(Vector2 center, float radius)
        {
            return WithCondition((NPC npc) => Vector2.DistanceSquared(npc.Center, center) < radius * radius);
        }

        /// <summary>
        /// Ensures the NPC is outside of some radius around a point.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public NPCConditions OutsideOfRadius(Vector2 center, float radius)
        {
            return WithCondition((NPC npc) => Vector2.DistanceSquared(npc.Center, center) >= radius * radius);
        }

        /// <summary>
        /// Ensures the NPC is not a target dummy
        /// </summary>
        /// <returns></returns>
        public NPCConditions NotTargetDummy()
        {
            return WithCondition((NPC npc) => npc.type != NPCID.TargetDummy);
        }

        /// <summary>
        /// Checks that the NPC is not in the list of excluded NPCs.
        /// </summary>
        /// <param name="excludedNPCs"></param>
        /// <returns></returns>
        public NPCConditions Exclude(int[] excludedNPCs)
        {
            return WithCondition((NPC npc) => !ArrayContains(excludedNPCs, npc.whoAmI));
        }

        private static bool ArrayContains<T>(T[] array, T item)
        {
            if (array != null)
            {
                foreach (T element in array)
                {
                    if (item.Equals(element)) return true;
                }
            }
            return false;
        }
    }
}
