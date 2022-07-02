using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using RoRMod.Content.Projectiles;
using RoRMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RoRMod.Content.Items.Accessories
{
    internal class Spikestrip : ModItem
    {
        public const float DetectionRadius = 250f;
        public const float SlowAmount = 0.1f;
        public const int SlowDuration = 30;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Release spikes when near enemies");
            ItemRaritySystem.CommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ModContent.RarityType<Rarities.CommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpikestripPlayer>().SetAccessory(Item, hideVisual);
        }
    }

    internal class SpikestripPlayer : ReleaseNearEnemyAccessoryPlayer
    {
        private CooldownTimer spikeTimer = new CooldownTimer(CooldownTimer.ResetMode.Automatic, 60);
        protected override CooldownTimer releaseTimer => spikeTimer;
        protected override float DetectionRadius => Spikestrip.DetectionRadius;

        protected override void ReleaseProjectile()
        {
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                // Release 3 spikes at random angles
                IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                int projectileType = ModContent.ProjectileType<SpikeProjectile>();
                for (int i = 0; i < 3; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * 8f;
                    Projectile.NewProjectile(spawnSource, Player.Center, velocity, projectileType, PlayerUtils.BaseDamage(Player) / 2, 1f, Player.whoAmI);
                }
            }

            SoundEngine.PlaySound(SoundID.Item1.WithVolumeScale(0.75f), Player.Center);
        }
    }
}
