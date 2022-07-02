using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace RoRMod.Content.Common
{
    internal class ItemRaritySystem : ModSystem
    {
        public static readonly List<int> CommonItems = new List<int>();
        public static readonly List<int> UncommonItems = new List<int>();
        public static readonly List<int> RareItems = new List<int>();
        public static readonly List<int> BossItems = new List<int>();

        public override void Unload()
        {
            CommonItems.Clear();
            UncommonItems.Clear();
            RareItems.Clear();
            BossItems.Clear();
        }
    }

    internal class BossItemDropNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // ~15% chance for boss to drop one of the boss items
            IItemDropRule bossDrop = ItemDropRule.OneFromOptions(7, ItemRaritySystem.BossItems.ToArray());
            
            if (System.Array.IndexOf(new int[] { NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail }, npc.type) > -1)
            {
                // EOW boss death condition
                LeadingConditionRule leadingConditionRule = new(new Conditions.LegacyHack_IsABoss());
                leadingConditionRule.OnSuccess(bossDrop);
                npcLoot.Add(leadingConditionRule);
            }
            else if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
            {
                // Twins boss death condition
                LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.MissingTwin());
                leadingConditionRule.OnSuccess(bossDrop);
                npcLoot.Add(leadingConditionRule);
            }
            else if (npc.boss && npc.realLife < 0)
            {
                npcLoot.Add(bossDrop);
            }
        }
    }
}
