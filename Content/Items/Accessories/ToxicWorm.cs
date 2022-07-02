using Terraria;
using Terraria.ModLoader;
using RoRMod.Utilities;
using Terraria.DataStructures;
using RoRMod.Content.Common;

namespace RoRMod.Content.Items.Accessories
{
    internal class ToxicWorm : ModItem
    {
        public const int PosionDuration = 180;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Spawns a worm that seeks out and poisons nearby enemies");
            ItemRaritySystem.UncommonItems.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.buyPrice(0, 5);
            Item.rare = ModContent.RarityType<Rarities.UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ToxicWormPlayer>().SetAccessory(Item, hideVisual);
        }
    }


    internal class ToxicWormPlayer : AccessoryPlayer
    {
        private CooldownTimer spawnTimer = new CooldownTimer(CooldownTimer.ResetMode.Automatic, 180);
        private UniqueProjectileTracker worm = new UniqueProjectileTracker(ModContent.ProjectileType<Projectiles.ToxicWormProjectile>());

        public override void UpdateEquips()
        {
            if (AccessoryEquipped && AccessoryVisible)
            {
                // Try to spawn Toxic Worm every 3 seconds
                spawnTimer.Tick();
                if (spawnTimer.OffCooldown && !worm.Alive)
                {
                    // Spawn new worm
                    IEntitySource spawnSource = Player.GetSource_Accessory(Accessory);
                    worm.Spawn(Player, spawnSource, Player.Center, Microsoft.Xna.Framework.Vector2.Zero, 1, 0);
                }
            }
            else
            {
                worm.Kill();
            }
        }
    }
}
