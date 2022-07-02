using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
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
    internal class LaserTurbineDevice : ModProjectile
    {
        public const int LaserDuration = 180;

        private int Threshold => Items.Accessories.LaserTurbine.Threshold;

        public enum Phase { Quarter1, Quarter2, Quarter3, Quarter4, Firing }

        private int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private Phase AIPhase
        {
            get { return (Phase)Projectile.ai[1]; }
            set { Projectile.ai[1] = (float)value; }
        }
        private int Charge 
        { 
            get { return (int)Projectile.localAI[0]; }
            set { Projectile.localAI[0] = value; }
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(Charge);
        public override void ReceiveExtraAI(BinaryReader reader) => Charge = reader.ReadInt32();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LaserTurbineDevice");

            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 46;
            Projectile.scale = 0.8f;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            // Kill if player died/left
            if (!Main.player[Projectile.owner].active)
            {
                Projectile.Kill();
                return;
            }

            // Always stay on top of player
            Projectile.Center = Main.player[Projectile.owner].Top - Vector2.UnitY * 35;

            switch (AIPhase)
            {
                case Phase.Quarter1:
                    Quarter1();
                    break;
                case Phase.Quarter2:
                    Quarter2();
                    break;
                case Phase.Quarter3:
                    Quarter3();
                    break;
                case Phase.Quarter4:
                    Quarter4();
                    break;
                case Phase.Firing:
                    Firing();
                    break;
            }

            Projectile.timeLeft = 2;
            Timer++;
        }

        private void Quarter1()
        {
            Hover();
            BlinkFrames(0, 1);

            if (Charge > 0.25f * Threshold)
            {
                AIPhase = Phase.Quarter2;
                SoundEngine.PlaySound(RoRSound.ChargeUp, Projectile.Center);
            }
        }

        private void Quarter2()
        {
            Hover();
            BlinkFrames(1, 2);

            if (Charge < 0.20f * Threshold)
            {
                AIPhase = Phase.Quarter1;
                SoundEngine.PlaySound(RoRSound.ChargeDown, Projectile.Center);
            }
            if (Charge > 0.5f * Threshold)
            {
                AIPhase = Phase.Quarter3;
                SoundEngine.PlaySound(RoRSound.ChargeUp.WithPitchOffset(0.25f), Projectile.Center);
            }
        }

        private void Quarter3()
        {
            Hover();
            BlinkFrames(2, 3);

            if (Charge < 0.45f * Threshold)
            {
                AIPhase = Phase.Quarter2;
                SoundEngine.PlaySound(RoRSound.ChargeDown.WithPitchOffset(0.25f), Projectile.Center);
            }
            if (Charge > 0.75f * Threshold)
            {
                AIPhase = Phase.Quarter4;
                SoundEngine.PlaySound(RoRSound.ChargeUp.WithPitchOffset(0.5f), Projectile.Center);
            }
        }

        private void Quarter4()
        {
            Hover();
            BlinkFrames(3, 4);

            if (Charge < 0.70f * Threshold)
            {
                AIPhase = Phase.Quarter3;
                SoundEngine.PlaySound(RoRSound.ChargeDown.WithPitchOffset(0.5f), Projectile.Center);
            }
        }

        private void Firing()
        {
            float progress = (float)Timer / LaserDuration;
            if (progress < 0.25f)
            {
                Projectile.frame = 4;
            }
            else if (progress < 0.5f)
            {
                Projectile.frame = 3;
            }
            else if (progress < 0.75f)
            {
                Projectile.frame = 2;
            }
            else if (progress < 1f)
            {
                Projectile.frame = 1;
            }
            else
            {
                Projectile.frame = 0;
            }

            // Dust
            if (Main.rand.NextBool())
            {
                Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
                Dust.NewDustPerfect(Projectile.Center, DustID.MagicMirror, velocity, newColor: Color.Red, Scale: 2f);
            }

            if (Timer >= LaserDuration)
            {
                Timer = 0;
                AIPhase = Phase.Quarter1;
            }
        }

        private void Hover()
        {
            // Drift up and down
            Projectile.Center += Vector2.UnitY * 5 * MathF.Sin(MathHelper.Pi * Timer / 60f);
        }

        private void BlinkFrames(int frame1, int frame2)
        {
            if (Timer % 60 < 30)
            {
                Projectile.frame = frame1;
            }
            else
            {
                Projectile.frame = frame2;
            }
        }

        public void UpdateCharge(int charge)
        {
            Charge = charge;
        }

        public void FireLasers(int damage)
        {
            if (PlayerUtils.IsOwnerClient(Projectile.owner))
            {
                int uuid = Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI);
                int projectileType = ModContent.ProjectileType<LaserTurbineLaser>();
                IEntitySource spawnSource = Projectile.GetSource_FromThis();
                // Right Beam:
                Projectile.NewProjectile(spawnSource, Projectile.Center, Vector2.UnitX, projectileType,
                    damage, 0, Projectile.owner, ai0: uuid);
                // Left Beam:
                Projectile.NewProjectile(spawnSource, Projectile.Center, -Vector2.UnitX, projectileType,
                    damage, 0, Projectile.owner, ai0: uuid);
                Projectile.netUpdate = true;
            }

            // Moon Lord Laser sound (Zombie104)
            SoundStyle style = SoundID.Zombie104;
            SoundEngine.PlaySound(style, Projectile.Center);
            Timer = 0;
            AIPhase = Phase.Firing;
        }
    }
}
