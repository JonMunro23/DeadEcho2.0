public interface IDamageable
{
    void OnDamaged(int damageTaken, string hitBodyPart);

    void Kill();
}
