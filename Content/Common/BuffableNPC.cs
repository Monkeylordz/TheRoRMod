using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using RoRMod.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;

namespace RoRMod.Content.Common
{
    internal class BuffableNPC : GlobalNPC, IDamageable
    {
        public override bool InstancePerEntity => true;
        private NPC NPC;

        public Entity Entity => NPC;
        public int Life
        {
            get => NPC.life;
            set => NPC.life = value;
        }
        public int MaxLife
        {
            get => NPC.lifeMax;
            set => NPC.lifeMax = value;
        }
        public int Defense
        {
            get => NPC.defense;
            set => NPC.defense = value;
        }
        public void Damage(int damage)
        {
            NPC mainNPC = NPC;
            if (NPC.realLife >= 0)
            {
                mainNPC = Main.npc[NPC.realLife];
            }
            if (!mainNPC.immortal)
            {
                mainNPC.life -= damage;
            }
            CombatText.NewText(NPC.Hitbox, CombatText.LifeRegenNegative, damage, dramatic: false, dot: true);
            mainNPC.checkDead();
            mainNPC.netUpdate = true;
        }

        public BuffManager Buffs { get; private set; }

        public override void SetDefaults(NPC npc)
        {
            NPC = npc;
            Buffs = new BuffManager();
        }

        public override bool PreAI(NPC npc)
        {
            Buffs.Update();

            // Freeze/Stun: disable AI
            bool useAI = !Buffs.HasBuff<FreezeBuff>() && !Buffs.HasBuff<StunBuff>();
            return useAI;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Buffs.Draw(NPC.Top + new Vector2(0, npc.gfxOffY - 24));
        }

        public override void OnKill(NPC npc)
        {
            Buffs.Clear();
        }
    }
}
