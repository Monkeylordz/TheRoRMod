using Microsoft.Xna.Framework;
using RoRMod.Utilities;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Projectiles
{
    internal class FrostRelicProjectile : ModProjectile
    {
        public const float RotationSpeed = 0.1f;
        public const float p = 20;
        public const float q = 40;

        private float Theta
        {
            get { return Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }

        private float Scale
        {
            get { return Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }

        private float Offset
        {
            get { return Projectile.localAI[0]; }
            set { Projectile.localAI[0] = value; }
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(Offset);
        public override void ReceiveExtraAI(BinaryReader reader) => Offset = reader.ReadSingle();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icicle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 2f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200; // 20s
            Projectile.penetrate = 3;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Randomize rotation and rose pattern constants
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Scale = Main.rand.NextFloat(200, 300);
            Offset = Main.rand.NextFloat(MathHelper.TwoPi);
            Theta = Main.rand.NextFloat(MathHelper.TwoPi * 2);
        }

        public override void AI()
        {
            // Kill if player died/left or Frost Relic unequipped
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || !owner.GetModPlayer<Items.Accessories.FrostRelicPlayer>().AccessoryEquipped)
            {
                Projectile.Kill();
                return;
            }

            // Move in mathematical rose pattern
            Rose();

            // Rotation
            Projectile.rotation += RotationSpeed;

            // Dust
            if (Main.rand.NextBool(4))
            {
                int d = Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.Ice);
                Main.dust[d].noGravity = true;
            }
        }

        private void Rose()
        {
            // Move in a mathmatical rose pattern with 3:2 ratio (6 petals)
            // r = a*cos(k*theta + b), k = 3/2
            float r = Scale * MathF.Cos(1.5f * Theta + Offset);
            float x = r * MathF.Cos(Theta);
            float y = r * MathF.Sin(Theta);
            Projectile.Center = new Vector2(x, y) + Main.player[Projectile.owner].Center;

            // Step = theta/frame
            // At petal periapsides, step = 1/p
            // At petal apoapsides, step = 1/(p+q)
            float step = Scale / (Scale * p + MathF.Abs(r) * q);
            Theta += step;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.FrostHydra);
            }
            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
        }
    }
}
