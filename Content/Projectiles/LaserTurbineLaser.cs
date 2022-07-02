using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Enums;
using Terraria.GameContent;

namespace RoRMod.Content.Projectiles
{
    internal class LaserTurbineLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LastPrismLaser;

        private const float MaxLength = 1000f;
        private const float TileCollisionWidth = 1f;
        private const float HitboxCollisionWidth = 22f;
        private const int NumSamplePoints = 3;
        private const float LengthChangeFactor = 0.75f;
        private const float LightBrightness = 0.5f;
        private Color LaserColor = Color.OrangeRed;

        private static Texture2D laserTexture = TextureAssets.Projectile[ProjectileID.LastPrismLaser].Value;

        private int HostIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float LaserLength
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser");

            // laserTexture = ModContent.Request<Texture2D>(Texture).Value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.scale = 4;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = LaserTurbineDevice.LaserDuration;
            Projectile.penetrate = -1;
            Projectile.maxPenetrate = -1;
        }

        public override void AI()
        {
            Projectile host = Main.projectile[HostIndex];
            if (Projectile.type != ModContent.ProjectileType<LaserTurbineLaser>() || host.type != ModContent.ProjectileType<LaserTurbineDevice>() || !host.active)
            {
                Projectile.Kill();
                return;
            }

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.Center = host.Center;
            // Add a forwards offset
            Projectile.position += direction * 16f;

            // Rotate a fixed amount each frame
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi / LaserTurbineDevice.LaserDuration);
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Hitscan
            float hitscanLaserLength = PerformLaserHitscan(host);
            LaserLength = MathHelper.Lerp(LaserLength, hitscanLaserLength, LengthChangeFactor);
            Vector2 hitboxStats = new Vector2(Projectile.velocity.Length() * LaserLength, Projectile.width * Projectile.scale);
            
            // Dust
            ProduceDust();

            // Disturb water
            if (Main.netMode != NetmodeID.Server)
            {
                ProduceWaterRipples(hitboxStats);
            }

            // Cast Light
            DelegateMethods.v3_1 = LaserColor.ToVector3() * LightBrightness;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * LaserLength, hitboxStats.Y, DelegateMethods.CastLight);
        }

        private float PerformLaserHitscan(Projectile host)
        {
            Vector2 samplingPoint = host.Center;
            float[] laserScanResults = new float[NumSamplePoints];
            Collision.LaserScan(samplingPoint, Projectile.velocity, TileCollisionWidth, MaxLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;

            return averageLengthSample;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Avoids calculations for extremely close targets
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }

            // Otherwise, perform an AABB line collision check.
            float _ = float.NaN;
            Vector2 endPos = Projectile.Center + Projectile.velocity * LaserLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPos, HitboxCollisionWidth * Projectile.scale, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = laserTexture;
            Vector2 centerFloored = Projectile.Center.Floor() + Projectile.velocity * Projectile.scale * 10.5f;
            Vector2 drawScale = new Vector2(Projectile.scale);

            // Reduce the length to reduce block penetration.
            float visualLength = LaserLength - 3 * Projectile.scale * Projectile.scale;

            DelegateMethods.f_1 = 1f; // f_1 is an unnamed decompiled variable whose function is unknown. Leave it at 1.
            Vector2 startPosition = centerFloored - Main.screenPosition;
            Vector2 endPosition = startPosition + Projectile.velocity * visualLength;

            // Draw Outer Laser
            DrawLaser(texture, startPosition, endPosition, drawScale, LaserColor);

            // Draw Inner Laser
            Color innerColor = LaserColor;
            innerColor.G = (byte)(innerColor.G / 2);
            innerColor.B = (byte)(innerColor.B / 2);
            DrawLaser(texture, startPosition, endPosition, drawScale * 0.5f, innerColor);

            return false;
        }

        public void DrawLaser(Texture2D texture, Vector2 startPosition, Vector2 endPosition, Vector2 drawScale, Color laserColor)
        {
            Utils.LaserLineFraming lineFraming = new Utils.LaserLineFraming(DelegateMethods.RainbowLaserDraw);

            // c_1 is an unnamed decompiled variable which is the render color of the beam drawn by DelegateMethods.RainbowLaserDraw.
            DelegateMethods.c_1 = laserColor; 
            Utils.DrawLaser(Main.spriteBatch, texture, startPosition, endPosition, drawScale, lineFraming);
        }

        private void ProduceDust()
        {
            // Create one dust per frame a small distance from where the beam ends.
            Vector2 endPosition = Projectile.Center + Projectile.velocity * (LaserLength - 14.5f * Projectile.scale);
            float angle = Projectile.rotation + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
            float startDistance = Main.rand.NextFloat(1f, 1.8f);
            float scale = Main.rand.NextFloat(0.7f, 1.1f);
            Vector2 velocity = angle.ToRotationVector2() * startDistance;
            Dust dust = Dust.NewDustDirect(endPosition, 0, 0, DustID.MagicMirror, velocity.X, velocity.Y, 0, LaserColor, scale);
            dust.color = LaserColor;
            dust.noGravity = true;
            dust.velocity *= Projectile.scale / 2;
            dust.scale *= Projectile.scale / 4;
        }

        private void ProduceWaterRipples(Vector2 beamDims)
        {
            WaterShaderData shaderData = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();

            // A universal time-based sinusoid which updates extremely rapidly. GlobalTime is 0 to 3600, measured in seconds.
            float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
            Vector2 ripplePos = Projectile.position + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(Projectile.rotation);

            Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
            shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation);
        }
    }
}