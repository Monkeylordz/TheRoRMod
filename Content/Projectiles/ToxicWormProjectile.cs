using Microsoft.Xna.Framework;
using RoRMod.Utilities;
using RoRMod.Content.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using System;
using Terraria.DataStructures;
using RoRMod.Content.Common;
using Terraria.Audio;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Projectiles
{
    internal class ToxicWormProjectile : ModProjectile
    {
        public const float OrbitRadius = 75f;
        public const float OrbitSpeed = 8f;
        public const int HomingDelay = 15;
        public const float DetectionRadius = 300f;
        public const float HomingSpeed = 16f;

        public const int FrameCount = 4;
        public const int AnimationDelay = 8;

        public enum Phase { Orbit, Homing }

        private int Timer
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        private Phase AIPhase
        {
            get { return (Phase)Projectile.ai[1]; }
            set { Projectile.ai[1] = (float)value; }
        }

        private int TargetNPC { get; set; }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(TargetNPC);
        public override void ReceiveExtraAI(BinaryReader reader) => TargetNPC = reader.ReadInt32();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxic Worm");

            Main.projFrames[Projectile.type] = FrameCount;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(RoRSound.ToxicWormSpawn.WithVolumeScale(0.5f), Projectile.Center);
        }

        public override void AI()
        {
            // Kill if player died/left
            if (!Main.player[Projectile.owner].active)
            {
                Projectile.Kill();
                return;
            }

            switch (AIPhase)
            {
                case Phase.Orbit:
                    Orbit();
                    break;
                case Phase.Homing:
                    Homing();
                    break;
            }

            // Animate
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationDelay)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                Projectile.frame %= FrameCount;
            }

            // Dust
            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.ChlorophyteWeapon);
                Main.dust[d].noGravity = true;
            }

            // Stay alive forever
            Projectile.timeLeft = 2;

            Timer++;
        }

        private void Orbit()
        {
            // Orbit around player
            Player player = Main.player[Projectile.owner];
            float theta = OrbitSpeed * MathHelper.ToRadians(Timer);
            Vector2 orbitPosition = OrbitRadius * new Vector2(MathF.Cos(theta), MathF.Sin(theta));
            Projectile.Center = player.Center + orbitPosition;
            Projectile.rotation = theta + MathHelper.PiOver2;

            // Search for nearby enemy
            if (Timer > HomingDelay && Timer % 10 == 0)
            {
                NPC nearestNPC = NPCUtils.GetClosestNPC(player.Center, 
                    NPCConditions.Base.Hostile().CanBeChased().WithinRadius(player.Center, DetectionRadius));
                if (nearestNPC != null)
                {
                    TargetNPC = nearestNPC.whoAmI;
                    AIPhase = Phase.Homing;
                    SoundEngine.PlaySound(RoRSound.ToxicWormLaunch.WithVolumeScale(0.5f), Projectile.position);
                }
            }
        }

        private void Homing()
        {
            NPC target = Main.npc[TargetNPC];

            // Check if target still alive
            if (!target.active)
            {
                Projectile.Kill();
            }
            
            // Home in toward target
            Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            Projectile.velocity = direction * HomingSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Poison for 50% base damage for 3 seconds
            BuffableNPC buffableNPC = target.GetGlobalNPC<BuffableNPC>();
            buffableNPC.Buffs.AddBuff(new PoisonBuff(buffableNPC, PlayerUtils.BaseDamage(Main.player[Projectile.owner], true) / 2, ToxicWorm.PosionDuration));
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.TopLeft, Projectile.width, Projectile.height, DustID.ChlorophyteWeapon);
            }

            SoundEngine.PlaySound(RoRSound.ToxicWormDeath.WithVolumeScale(0.5f), Projectile.Center);
        }
    }
}
