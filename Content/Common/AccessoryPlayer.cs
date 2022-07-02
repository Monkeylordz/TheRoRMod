using Microsoft.Xna.Framework;
using RoRMod.Content.Buffs;
using RoRMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RoRMod.Content.Common
{
    internal abstract class AccessoryPlayer : ModPlayer
    {
        private bool equippedLastFrame;
        public Item Accessory { get; private set; }
        public bool AccessoryEquipped { get; private set; }
        public bool AccessoryVisible { get; private set; }
        public bool JustEquipped => AccessoryEquipped && !equippedLastFrame;

        public void SetAccessory(Item accessory, bool hideVisual = false)
        {
            Accessory = accessory;
            AccessoryEquipped = true;
            AccessoryVisible = !hideVisual;
        }

        public override void ResetEffects()
        {
            equippedLastFrame = AccessoryEquipped;
            AccessoryEquipped = false;
            AccessoryVisible = false;
        }
    }

    internal abstract class ModifyHitAccessoryPlayer : AccessoryPlayer
    {
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            ModifyHit(target, ref damage, ref knockback, ref crit);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            ModifyHit(target, ref damage, ref knockback, ref crit);
        }

        public abstract void ModifyHit(NPC target, ref int damage, ref float knockback, ref bool crit);
    }

    internal abstract class OnHitAccessoryPlayer : AccessoryPlayer
    {
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            OnHit(target, damage, knockback, crit);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            OnHit(target, damage, knockback, crit);
        }

        public abstract void OnHit(NPC target, int damage, float knockback, bool crit);
    }

    internal abstract class OnHitAtPositionAccessoryPlayer : AccessoryPlayer
    {
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            Vector2 hitPosition = target.Hitbox.ClosestPointInRect(Player.Center);
            OnHit(target, hitPosition, damage, knockback, crit);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            Vector2 hitPosition = proj.Center;
            OnHit(target, hitPosition, damage, knockback, crit);
        }

        public abstract void OnHit(NPC target, Vector2 hitPosition, int damage, float knockback, bool crit);
    }

    internal abstract class OnKillAccessoryPlayer : AccessoryPlayer
    {
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0) OnKill(target, damage, knockback, crit);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0) OnKill(target, damage, knockback, crit);
        }

        public abstract void OnKill(NPC target, int damage, float knockback, bool crit);
    }

    internal abstract class OnGetHitAccessoryPlayer : AccessoryPlayer
    {
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            OnGetHit(npc, damage, crit);
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            OnGetHit(Main.npc[proj.owner], damage, crit);
        }

        public abstract void OnGetHit(NPC npc, int damage, bool crit);
    }

    internal abstract class ShootAccessoryPlayer : AccessoryPlayer
    {
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            OnShoot(source, position, velocity, type, damage, knockback);
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }

        protected abstract void OnShoot(EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback);
    }

    internal abstract class ReleaseNearEnemyAccessoryPlayer : AccessoryPlayer
    {
        protected abstract CooldownTimer releaseTimer { get; }
        protected abstract float DetectionRadius { get; }

        public override void UpdateEquips()
        {
            if (AccessoryEquipped)
            {
                releaseTimer.Tick();
                if (releaseTimer.OffCooldown && PlayerUtils.IsNearHostileNPC(Player, DetectionRadius))
                {
                    ReleaseProjectile();
                }
            }
        }

        protected abstract void ReleaseProjectile();
    }

    internal abstract class IndicatorBuffAccessoryPlayer<T> : AccessoryPlayer where T : Buff
    {
        protected BuffManager playerBuffs;

        public override void Initialize()
        {
            playerBuffs = Player.GetModPlayer<BuffablePlayer>().Buffs;
        }

        protected abstract T GetBuff();

        protected void ApplyBuff()
        {
            playerBuffs.AddBuff(GetBuff());
        }

        protected void ClearBuff()
        {
            playerBuffs.ClearBuff<T>();
        }
    }
}
