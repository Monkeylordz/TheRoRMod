using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class TheHitList : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Non-boss enemies have a chance to be marked\n" +
                "Marked enemies take 50% more damage and drop more money");
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
            TheHitListPlayer.TheHitListEquipped = true;
        }
    }


    internal class TheHitListPlayer : ModPlayer
    {
        public static bool TheHitListEquipped = false;

        public override void ResetEffects()
        {
            TheHitListEquipped = false;
        }
    }


    internal class TheHitListInstancedNPC : GlobalNPC
    {
        private const string MarkerTexturePath = "RoRMod/Content/Items/Accessories/TheHitList_Marker";
        private static Asset<Texture2D> markerTextureAsset;

        public override bool InstancePerEntity => true;

        private bool marked;
        private float markerRotation;

        public override void SetStaticDefaults()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                markerTextureAsset = ModContent.Request<Texture2D>(MarkerTexturePath);
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            // 50% chance to mark non-boss, hostile NPCs on spawn
            if (TheHitListPlayer.TheHitListEquipped && !npc.friendly && !npc.boss && npc.type != NPCID.TargetDummy && Main.rand.NextBool())
            {
                marked = true;

                // Randomly set rotation
                markerRotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
        }

        public override bool PreAI(NPC npc)
        {
            // Remove marker if unequipped
            if (marked && !TheHitListPlayer.TheHitListEquipped)
            {
                marked = false;
            }
            return true;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (marked)
            {
                if (markerTextureAsset == null) return;

                Texture2D markerTexture = markerTextureAsset.Value;

                // Center marker on NPC center
                Vector2 textureCenter = markerTexture.Size() / 2;
                Vector2 position = npc.Center - screenPos + new Vector2(0, npc.gfxOffY);

                // Scale slightly to NPC scale
                float scale = 1;
                if (markerTexture.Width > 0)
                {
                    scale = Math.Min(npc.width, npc.height) / markerTexture.Width;
                    scale = scale < 1 ? 1 : scale;
                }

                // Set marker minimum brightness
                byte minColorValue = 140;
                if (drawColor.R < minColorValue) drawColor.R = minColorValue;
                if (drawColor.G < minColorValue) drawColor.G = minColorValue;
                if (drawColor.B < minColorValue) drawColor.B = minColorValue;

                Main.EntitySpriteDraw(markerTexture, position, null, drawColor, markerRotation, textureCenter, scale, SpriteEffects.None, 0);
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            ModifyOnHit(npc, ref damage);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            ModifyOnHit(npc, ref damage);
        }

        private void ModifyOnHit(NPC npc, ref int damage)
        {
            // 50% more damage taken
            if (marked)
            {
                damage = (int)(damage * 1.5f);
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Drop 50% more money
            if (marked)
            {
                npc.value *= 1.5f;
            }
        }

        public override void OnKill(NPC npc)
        {
            if (marked)
            {
                // Sound
                SoundEngine.PlaySound(RoRSound.HitListKill.WithVolumeScale(0.33f), npc.Center);
            }
        }
    }
}
