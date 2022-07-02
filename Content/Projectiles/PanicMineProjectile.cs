using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using RoRMod.Utilities;
using Terraria.Audio;

namespace RoRMod.Content.Projectiles
{
    internal class PanicMineProjectile : ModProjectile
    {
        public const int StoppingTime = 120;
        public const float BlastRadius = 50f;

        private const int MaxFrameTime = 15;

        protected virtual float Speed => Items.Accessories.PanicMines.MineSpeed;

        private int Timer
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Panic Mine");

            Main.projFrames[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Bounce on tiles
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Create dust cone
            float blastDirection = (Projectile.Center - target.Center).ToRotation();
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit(blastDirection, MathHelper.PiOver2) * (Main.rand.NextFloat() + 1) * 3f;
                Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.HeatRay, SpeedX: velocity.X, SpeedY: velocity.Y);
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int expand = (int)BlastRadius;
            hitbox.Inflate(expand, expand);
        }

        public override void AI()
        {
            // Slow to stop
            if (Timer <= StoppingTime)
            {
                float progress = MathHelper.Clamp((float)Timer / StoppingTime, 0, 1);
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Speed * (1 - progress);
                Timer++;
            }

            // Animate 
            if (Projectile.frameCounter > MaxFrameTime)
            {
                // alternate between frames 0 and 1
                Projectile.frameCounter = 0;
                Projectile.frame ^= 1;
            }
            Projectile.frameCounter++;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.Smoke);
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
        }
    }
}
