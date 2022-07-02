using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class DamageProjectile : ModProjectile
    {
        private int NPCID
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Damage");
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1;

            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.whoAmI == NPCID;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
