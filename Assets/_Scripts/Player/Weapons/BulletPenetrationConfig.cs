using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Bullet Penetration Config", menuName = "Weapons/Bullet Penetration Config")]
public class BulletPenetrationConfig : ScriptableObject
{
    public int MaxObjectsToPenetrate = 0;
    public float MaxPenetrationDepth = 0.275f;
    public float DamageRetentionPercentage;
}
