using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class LegendarySparkDamageProjectile : DamageProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Smite");
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Dust
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(target.TopLeft, target.width, target.height, DustID.GoldFlame);
            }
                
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}
