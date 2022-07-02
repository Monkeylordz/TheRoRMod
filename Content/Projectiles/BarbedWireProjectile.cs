using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using RoRMod.Content.Items.Accessories;

namespace RoRMod.Content.Projectiles
{
    internal class BarbedWireProjectile : ModProjectile
    {
        public const int SpriteSize = 128;
        public const int WireClearance = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barbed Wire");
        }

        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.scale = 2 * BarbedWire.Radius / SpriteSize;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;

            DrawOffsetX = -SpriteSize / 2;
            DrawOriginOffsetY = -SpriteSize / 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void AI()
        {
            // Kill if player died/left
            if (!Main.player[Projectile.owner].active)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = Main.player[Projectile.owner].Center;
            Projectile.rotation += 0.01f;

            // Stay alive forever
            Projectile.timeLeft = 2;
        }
    }
}
