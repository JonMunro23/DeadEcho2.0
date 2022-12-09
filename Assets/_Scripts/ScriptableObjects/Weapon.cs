using UnityEngine;

[CreateAssetMenu(fileName = "WeaponObject", menuName = "New Weapon")]
public class Weapon : ScriptableObject
{
    public enum WeaponSlotType
    {
        primary,
        secondary
    };
    public WeaponSlotType weaponSlotType;

    public new string name;
    [TextArea(3, 10)]
    public string description;
    public int cost;

    public GameObject weaponObj;
    public GameObject weaponWallBuyObj;
    public GameObject bulletHole;
    public GameObject muzzleEffect;

    public Sprite UISprite;

    public int magSize;
    public int maxReserveAmmo;
    public float reloadSpeed;
    public int projectileCount;
    public float maxSpreadDeviationAngle;
    public int damage;
    public float perShotCooldown;
    public float aimingFOV;
    public bool isAutomatic;
    public bool isProjectile;
    public GameObject projectile;
    public float blastRadius;
    public float projectileInitalVelocity;
    public GameObject projectileDestructionEffect;

    public AudioClip[] fireSFX;
    public AudioClip reloadSFX;
    public AudioClip equipSFX, unequipSFX;
}
