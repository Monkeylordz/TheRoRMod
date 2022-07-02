using Terraria;

namespace RoRMod.Content.Buffs
{
    public interface IDamageable
    {
        Entity Entity { get; }
        int Life { get; set; }
        int MaxLife { get; set; }
        int Defense { get; set; }
        void Damage(int damage);
    }
}
