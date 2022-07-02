using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RoRMod.Content.Items.Weapons;
using Terraria.DataStructures;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Projectiles
{
    internal class BrilliantBehemothBaseProjectile : ModProjectile
    {
        public const float ForwardOffsetScale = 100f;

        private int Charge
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private static int behemothItemType;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brilliant Behemoth");

            behemothItemType = ModContent.ItemType<BrilliantBehemoth>();

            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = BrilliantBehemoth.MaxCharge;

            DrawOffsetX = -16;
            DrawOriginOffsetY = -8;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(RoRSound.BehemothChargeStock.WithVolumeScale(0.5f), Projectile.Center);
            Projectile.frame = 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Check if player is alive / still using behemoth
            if (!player.active || player.HeldItem == null || player.HeldItem.type != behemothItemType)
            {
                Projectile.Kill();
                return;
            }

            // Set position/rotation
            Vector2 offset = new Vector2( -player.direction * 4, -4);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.velocity = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);
                if (Projectile.oldVelocity != Projectile.velocity)
                {
                    Projectile.netUpdate = true;
                }
            }
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * ForwardOffsetScale + offset;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Update player
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            if (!player.channel || Projectile.timeLeft <= 1)
            {
                // Fire on release / time out
                Fire();
            }
            else
            {
                // Charge up when held
                IncrementCharge();
            }
        }

        private void IncrementCharge()
        {
            Charge++;

            // Change frame at charge thresholds
            if (Charge == BrilliantBehemoth.ClusterThresholdCharge / 2)
            {
                Projectile.frame = 1;
                SoundEngine.PlaySound(RoRSound.BehemothChargeStock.WithVolumeScale(0.5f).WithPitchOffset(0.15f), Projectile.Center);
            }
            else if (Charge == BrilliantBehemoth.ClusterThresholdCharge)
            {
                Projectile.frame = 2;
                SoundEngine.PlaySound(RoRSound.BehemothChargeStock.WithVolumeScale(0.5f).WithPitchOffset(0.3f), Projectile.Center);
            }
        }

        private void Fire()
        {
            if (PlayerUtils.IsOwnerClient(Projectile.owner))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].Center,
                    Projectile.velocity * (BrilliantBehemoth.BaseShootSpeed + Charge * BrilliantBehemoth.ShootSpeedIncreaseMultiplier),
                    ModContent.ProjectileType<BrilliantBehemothProjectile>(),
                    Projectile.damage + (int)(Charge * BrilliantBehemoth.DamageIncreaseMultiplier), 
                    Projectile.knockBack, Projectile.owner, ai0: Charge);
            }
            SoundEngine.PlaySound(SoundID.Item61, Projectile.Center);
            Projectile.Kill();
        }
    }
}
