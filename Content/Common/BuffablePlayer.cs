using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using RoRMod.Content.Buffs;
using Microsoft.Xna.Framework;

namespace RoRMod.Content.Common
{
    internal class BuffablePlayer : ModPlayer, IDamageable
    {
		Entity IDamageable.Entity => Entity;
		public int Life
		{
			get => Player.statLife;
			set => Player.statLife = value;
		}
		public int MaxLife
		{
			get => Player.statLifeMax2;
			set => Player.statLifeMax2 = value;
		}
		public int Defense
		{
			get => Player.statDefense;
			set => Player.statDefense = value;
		}
		public void Damage(int damage)
		{
			if (!Player.immune)
			{
				Player.statLife -= damage;
			}
			CombatText.NewText(Player.Hitbox, CombatText.LifeRegenNegative, damage, dramatic: false, dot: true);
		}

		public BuffManager Buffs { get; private set; }

        public override void Initialize()
        {
			Buffs = new BuffManager();
		}

        public override void PostUpdateBuffs()
        {
			Buffs.Update();
		}

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
			// Draw above player
			Buffs.Draw(Player.Top + new Vector2(0, Player.gfxOffY - 24));
		}

        public override void SetControls()
        {
			// Freeze/Stun by stopping input
			if (Buffs.HasBuff<FreezeBuff>() || Buffs.HasBuff<StunBuff>())
			{
				Player.controlUp = false;
				Player.controlDown = false;
				Player.controlLeft = false;
				Player.controlRight = false;
				Player.controlJump = false;
			}
		}

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
			// Clear all buffs on death
			Buffs.Clear();
		}
    }
}
