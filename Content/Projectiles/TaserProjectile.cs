using Microsoft.Xna.Framework;
using RoRMod.Content.Items.Weapons;
using RoRMod.Utilities;
using RoRMod.Content.Common;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using RoRMod.Content.Buffs;

namespace RoRMod.Content.Projectiles
{
    public class TaserProjectile : ModProjectile
    {
        public const int NoGravityDuration = 15;

        enum Phase { Fly, Attach, Retract }

        private int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private Phase AIPhase
        {
            get => (Phase)Projectile.ai[1];
            set => Projectile.ai[1] = (int)value;
        }

        private int AttachedNPC
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(AttachedNPC);
        public override void ReceiveExtraAI(BinaryReader reader) => AttachedNPC = reader.ReadInt32();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Taser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 10;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            StartRetract();
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (AIPhase == Phase.Retract)
            {
                return false;
            }
            else if (AIPhase == Phase.Attach)
            {
                return target.whoAmI == AttachedNPC;
            }
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Attach to enemy
            if (AIPhase != Phase.Attach)
            {
                AIPhase = Phase.Attach;
                AttachedNPC = target.whoAmI;
                Projectile.tileCollide = false;
            }

            // Zap & Stun enemy
            Player player = Main.player[Projectile.owner];
            DustUtils.LightningBolt(player.Center, Projectile.Center);
            target.GetGlobalNPC<BuffableNPC>().Buffs.AddBuff(new StunBuff(target, Taser.StunDurationWeapon));
            SoundEngine.PlaySound(RoRSound.ChainLightning.WithVolumeScale(0.25f), target.Center);

            // Return on last penetrate hit
            if (Projectile.penetrate == 1)
            {
                StartRetract();
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            AttachedNPC = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // kill if player not alive
            if (!player.active)
            {
                Projectile.Kill();
            }

            // Retract if not channeled
            if (AIPhase != Phase.Retract && !player.channel)
            {
                StartRetract();
            }

            switch (AIPhase)
            {
                case Phase.Fly:
                    Fly();
                    break;
                case Phase.Attach:
                    Attach();
                    break;
                case Phase.Retract:
                    Retract();
                    break;
            }

            // Update player
            Vector2 playerToProj = Projectile.Center - player.Center;
            player.ChangeDir(Math.Sign(playerToProj.X));
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (playerToProj * player.direction).ToRotation();

            Projectile.timeLeft = 2;
            Timer++;
        }

        private void Fly()
        {
            if (Timer > NoGravityDuration)
            {
                // Gravity and Drag
                Projectile.velocity.Y = MathF.Min(Projectile.velocity.Y + 0.4f, 40);
                Projectile.velocity.X *= 0.99f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Retract if too far away
            if (Vector2.DistanceSquared(Projectile.Center, Main.player[Projectile.owner].Center) > Taser.MaxFlyDistance * Taser.MaxFlyDistance)
            {
                StartRetract();
            }
        }

        private void Attach()
        {
            // Stay attached to NPC
            NPC npc = Main.npc[AttachedNPC];
            if (!npc.active)
            {
                StartRetract();
                return;
            }
            Projectile.Center = npc.Center - Projectile.velocity;
            Projectile.gfxOffY = npc.gfxOffY;

            // Retract if too far away
            if (Vector2.DistanceSquared(Projectile.Center, Main.player[Projectile.owner].Center) > Taser.MaxAttachDistance * Taser.MaxAttachDistance)
            {
                StartRetract();
            }
        }

        private void Retract()
        {
            Player player = Main.player[Projectile.owner];

            // Kill when close enough to player
            Vector2 toPlayer = player.Center - Projectile.Center;
            if (toPlayer.LengthSquared() < Taser.RetractSpeed * Taser.RetractSpeed)
            {
                Projectile.Kill();
                return;
            }

            // Move toward player
            Projectile.velocity = toPlayer.SafeNormalize(Vector2.Zero) * Taser.RetractSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private void StartRetract()
        {
            AIPhase = Phase.Retract;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            AttachedNPC = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw line from projectile to player
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 0);
            Player player = Main.player[Projectile.owner];
            Vector2 position = player.MountedCenter + player.itemRotation.ToRotationVector2() * 30f * player.direction;
            Vector2 lineVector = Projectile.Center - position;
            float rotation = lineVector.ToRotation() - MathHelper.PiOver2;
            Vector2 scale = new Vector2(1, lineVector.Length() / frame.Height);
            Main.EntitySpriteDraw(texture, position - Main.screenPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0);
            
            return true;
        }
    }
}
