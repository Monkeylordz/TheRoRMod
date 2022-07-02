using Microsoft.Xna.Framework;
using RoRMod.Content.Buffs;
using RoRMod.Content.Common;
using RoRMod.Content.Items.Accessories;
using RoRMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class PoisonMineProjectile : PanicMineProjectile
    {
        protected override float Speed => DeadMansFoot.MineSpeed;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Posion Mine");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Poison for 50% damage for 3 seconds
            BuffableNPC buffableNPC = target.GetGlobalNPC<BuffableNPC>();
            buffableNPC.Buffs.AddBuff(new BurnBuff(buffableNPC, damage / 2, DeadMansFoot.PoisonDuration));

            // Dust
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f) * 2f;
                Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.VenomStaff, velocity.X, velocity.Y);
            }
        }
    }
}
