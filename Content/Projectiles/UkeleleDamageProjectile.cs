using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class UkeleleDamageProjectile : DamageProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Arc");
        }
    }
}
