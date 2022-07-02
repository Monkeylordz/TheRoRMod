using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class IfritsHornDamageProjectile : DamageProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Incineration");
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Dust
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(target.TopLeft, target.width, target.height, DustID.Torch);
            }
                
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}
