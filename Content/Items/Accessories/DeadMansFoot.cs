using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using RoRMod.Utilities;
using RoRMod.Content.Projectiles;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Items.Accessories
{
    internal class DeadMansFoot : ModItem
    {
        public const float MineSpeed = 3f;
        public const float DetectionRadius = 300f;
        public const int PoisonDuration = 180;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dead Man's Foot");
            Tooltip.SetDefault("Release poison mines when near enemies");
            ItemRaritySystem.UncommonItems.Add(Type);
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 56;
            Item.height = 56;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<DeadMansFootPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class DeadMansFootPlayer : ReleaseNearEnemyAccessoryPlayer
    {
        private CooldownTimer mineTimer = new CooldownTimer(CooldownTimer.ResetMode.Automatic, 60);
        protected override CooldownTimer releaseTimer => mineTimer;
        protected override float DetectionRadius => DeadMansFoot.DetectionRadius;

        protected override void ReleaseProjectile()
        {
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                // Shoot a Poison Mine at a random angle
                IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                int projectileType = ModContent.ProjectileType<PoisonMineProjectile>();
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f);
                Projectile.NewProjectile(spawnSource, Player.Center, velocity, projectileType, PlayerUtils.BaseDamage(Player), 1f, Player.whoAmI);
            }

            SoundEngine.PlaySound(RoRSound.MineSpawn.WithVolumeScale(0.25f).WithPitchVariance(0.1f), Player.Center);
        }
    }
}
