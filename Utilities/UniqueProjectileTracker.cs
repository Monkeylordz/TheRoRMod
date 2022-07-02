using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace RoRMod.Utilities
{
    internal class UniqueProjectileTracker
    {
        public int ProjectileType { get; private set; }
        public int ProjectileID { get; private set; }
        public Projectile Projectile => Main.projectile[ProjectileID];
        public bool Alive => 
            ProjectileID != -1 &&
            Projectile.active &&
            Projectile.type == ProjectileType;
        
        public UniqueProjectileTracker(int projectileType)
        {
            ProjectileType = projectileType;
            ProjectileID = -1;
        }

        public void Spawn(Player player, IEntitySource spawnSource, Vector2 position, Vector2 velocity, int damage, float knockback, float ai0 = 0, float ai1 = 0)
        {
            if (!Alive)
            {
                ProjectileID = Projectile.NewProjectile(spawnSource, position, velocity, ProjectileType, damage, knockback, player.whoAmI, ai0, ai1);
            }
        }

        public void Kill()
        {
            if (Alive)
            {
                Projectile.Kill();
            }
            ProjectileID = -1;
        }
    }
}
