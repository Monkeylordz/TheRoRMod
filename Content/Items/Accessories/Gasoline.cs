using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using Terraria.Audio;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class Gasoline : ModItem
    {
        public const float IgniteRadius = 200f;
        public const float BurnMultiplier = 0.6f;
        public const int BurnDuration = 180;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Enemies you kill ignite other nearby enemies");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 46;
            Item.height = 46;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GasolinePlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class GasolinePlayer : OnKillAccessoryPlayer
    {
        public override void OnKill(NPC target, int damage, float knockback, bool crit)
        {
            // Ignite on kill
            if (AccessoryEquipped)
            {
                GasolineIgnite(target);
            }
        }

        private void GasolineIgnite(NPC npc)
        {
            // Ignite all hostile NPCs within radius
            List<NPC> npcs = NPCUtils.GetNPCs(NPCConditions.Base.Hostile().WithinRadius(npc.Center, Gasoline.IgniteRadius));
            foreach (NPC target in npcs)
            {
                BuffableNPC buffableNPC = target.GetGlobalNPC<BuffableNPC>();
                buffableNPC.Buffs.AddBuff(new BurnBuff(buffableNPC, (int)(Gasoline.BurnMultiplier * PlayerUtils.BaseDamage(Player)), Gasoline.BurnDuration));
            }

            DustUtils.Explosion(npc.Center, Gasoline.IgniteRadius, DustID.Torch);

            SoundEngine.PlaySound(RoRSound.Gasoline.WithVolumeScale(0.75f), npc.Center);
        }
    }
}
