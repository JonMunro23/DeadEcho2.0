public interface IDamageable
{
    void OnDamaged(int damageTaken, string bodyPartTag);

    void InstantlyKill();
}
