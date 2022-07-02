using Terraria;
using Terraria.ID;
using RoRMod.Utilities;
using Terraria.DataStructures;
using RoRMod.Content.Common;
using Terraria.Audio;

namespace RoRMod.Content.Projectiles
{
    internal class BehemothExplosionProjectile : ExplosionProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Behemoth Explosion");
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(RoRSound.BehemothBlast.WithVolumeScale(0.2f), Projectile.position);
        }

        protected override void Explode()
        {
            DustUtils.Explosion(Projectile.Center, Radius, DustID.GoldFlame);
        }
    }
}
