using UnityEngine;

[CreateAssetMenu(fileName ="Bullet Penetration Data", menuName = "Weapons/Bullet Penetration Data")]
public class BulletPenetrationData : ScriptableObject
{
    public int MaxObjectsToPenetrate = 0;
    public float MaxPenetrationDepth = 0.275f;
    public float DamageRetentionPercentage;
}
