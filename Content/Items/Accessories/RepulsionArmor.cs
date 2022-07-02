using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Utilities;
using System;
using RoRMod.Content.Common;
using Terraria.Audio;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Items.Accessories
{
    internal class RepulsionArmor : ModItem
    {
        public const int ReflectDuration = 180;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("After being hit, reduce and reflect incoming damage for a short time");
            ItemRaritySystem.RareItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 56;
            Item.height = 56;
            Item.value = Item.buyPrice(0, 10);
            Item.rare = ModContent.RarityType<Rarities.RareRarity>();
            Item.defense = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<RepulsionArmorPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class RepulsionArmorPlayer : OnGetHitAccessoryPlayer
    {
        private BuffManager playerBuffs;

        public override void Initialize()
        {
            playerBuffs = Player.GetModPlayer<BuffablePlayer>().Buffs;
        }

        public override void UpdateEquips()
        {
            if (!AccessoryEquipped && playerBuffs.HasBuff<RepulsionArmorBuff>())
            {
                // Remove Repulsion Armor Buff on unequip
                playerBuffs.ClearBuff<RepulsionArmorBuff>();
            }
        }

        public override void OnGetHit(NPC npc, int damage, bool crit)
        {
            // Activate on hit
            if (AccessoryEquipped)
            {
                playerBuffs.AddBuff(new RepulsionArmorBuff(Player, RepulsionArmor.ReflectDuration));
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            ModifyGetHit(npc, ref damage, ref crit);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            ModifyGetHit(Main.npc[proj.owner], ref damage, ref crit);
        }

        public void ModifyGetHit(NPC npc, ref int damage, ref bool crit)
        {
            // Reduce and Reflect incoming damage
            if (AccessoryEquipped && playerBuffs.HasBuff<RepulsionArmorBuff>())
            {
                int incomingDamage = damage;
                damage = (int)(0.17f * incomingDamage);
                if (npc != null)
                {
                    int hitDirection = MathF.Sign(npc.Center.X - Player.Center.X);
                    Player.ApplyDamageToNPC(npc, incomingDamage, 3f, hitDirection, crit);
                }

                // Dust
                Dust.NewDust(Player.TopLeft, Player.width, Player.height, DustID.Titanium);

                // Sound
                SoundEngine.PlaySound(RoRSound.RepulsionArmorHit, Player.Center);
            }
        }
    }
}
