using Terraria;
using Terraria.ModLoader;
using RoRMod.Content.Common;
using RoRMod.Utilities;
using Microsoft.Xna.Framework;
using System;

namespace RoRMod.Content.Items.Accessories
{
    internal class Headstompers : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Increased jump height and fall speed\n" +
                "Grants the ability to stomp on enemies");
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
            player.jumpSpeedBoost += 1.2f;
            player.maxFallSpeed += 1.2f;
            player.extraFall += 15;
            player.GetModPlayer<HeadstompersPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class HeadstompersPlayer : AccessoryPlayer
    {
        public const float SpeedThreshold = 3f;
        public const float StompYBoost = -10f;

        private int[] npcImmunity = new int[Main.maxNPCs];

        public override void PreUpdateMovement()
        {
            // Only try to stomp if moving fast enough
            // TODO: Check mount?
			if (AccessoryEquipped && Player.velocity.Y > SpeedThreshold)
            {
                CheckEnemyBelow();

                // Local NPC immunity
                for (int i = 0; i < npcImmunity.Length; i++)
                {
                    if (npcImmunity[i] > 0)
                    {
                        npcImmunity[i]--;
                    }
                }
            }
		}

        private void CheckEnemyBelow()
        {
            // Check NPC collision
            Rectangle playerRect = Player.getRect();
            playerRect.Offset(0, Player.height);
            playerRect.Height = 2;
            playerRect.Inflate(8, 8);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.dontTakeDamage || npc.friendly || npcImmunity[npc.whoAmI] != 0 || npc.immune[Player.whoAmI] != 0 || !Player.CanNPCBeHitByPlayerOrPlayerProjectile(npc))
                {
                    continue;
                }
                Rectangle npcRect = npc.getRect();
                if (playerRect.Intersects(npcRect) && (npc.noTileCollide || Collision.CanHit(Player, npc)))
                {
                    // Enemy below: stomp
                    Stomp(npc);
                    break;
                }
            }
        }

        private void Stomp(NPC npc)
        {
            // Damage NPC for 150% base damage and boost upward
            if (Player.whoAmI == Main.myPlayer)
            {
                Player.ApplyDamageToNPC(npc, (int)(1.5f * PlayerUtils.BaseDamage(Player, true)), 5f, Math.Sign(Player.velocity.X), false);
            }
            npcImmunity[npc.whoAmI] = 10;
            Player.velocity.Y = StompYBoost;
            Player.GiveImmuneTimeForCollisionAttack(8);
        }
    }
}
