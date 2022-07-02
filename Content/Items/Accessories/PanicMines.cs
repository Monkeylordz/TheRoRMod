using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using RoRMod.Content.Projectiles;
using RoRMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RoRMod.Content.Items.Accessories
{
    internal class PanicMines : ModItem
    {
        public const float MineSpeed = 3f;
        public const float DetectionRadius = 300f;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Release panic mines when near enemies");
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
            player.GetModPlayer<PanicMinesPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class PanicMinesPlayer : ReleaseNearEnemyAccessoryPlayer
    {
        private CooldownTimer mineTimer = new CooldownTimer(CooldownTimer.ResetMode.Automatic, 60);
        protected override CooldownTimer releaseTimer => mineTimer;
        protected override float DetectionRadius => PanicMines.DetectionRadius;

        protected override void ReleaseProjectile()
        {
            if (PlayerUtils.IsOwnerClient(Player.whoAmI))
            {
                // Shoot 3 Panic Mines at random angles
                IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                int projectileType = ModContent.ProjectileType<PanicMineProjectile>();
                for (int i = 0; i < 3; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Projectile.NewProjectile(spawnSource, Player.Center, velocity, projectileType, PlayerUtils.BaseDamage(Player), 1f, Player.whoAmI);
                }
            }

            SoundEngine.PlaySound(RoRSound.MineSpawn.WithVolumeScale(0.25f).WithPitchVariance(0.1f), Player.Center);
        }
    }
}
