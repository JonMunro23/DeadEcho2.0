public interface IDamageable
{
    void OnDamaged(int damageTaken, bool wasHeadshot);

    void InstantlyKill();
}
