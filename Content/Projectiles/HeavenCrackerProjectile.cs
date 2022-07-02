using Microsoft.Xna.Framework;
using RoRMod.Content.Items.Weapons;
using RoRMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    public class HeavenCrackerProjectile : ModProjectile
    {
        public override string Texture => "RoRMod/Content/Items/Weapons/HeavenCracker";

        private int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heaven Cracker");
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = ProjAIStyleID.Drill;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
        }

        public override void PostAI()
        {
            base.PostAI();

            Timer++;

            // Fix rotation
            Projectile.rotation += MathHelper.PiOver2 * -Projectile.direction;

            // Create fissure
            if (Timer % HeavenCracker.FissureInterval == 0 && PlayerUtils.IsOwnerClient(Projectile.owner))
            {
                Player player = Main.player[Projectile.owner];
                int projectileType = ModContent.ProjectileType<HeavenCrackerFissureBaseProjectile>();
                Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(-Vector2.UnitY);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center + direction, direction,
                    projectileType, Projectile.damage, 1f, player.whoAmI);
            }
        }
    }
}
