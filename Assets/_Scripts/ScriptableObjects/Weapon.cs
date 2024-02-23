using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "WeaponObject", menuName = "New Weapon")]
public class Weapon : ScriptableObject
{
    public enum WeaponSlotType
    {
        primary,
        secondary
    };
    public WeaponSlotType weaponSlotType;

    public enum ReloadType
    {
        magazine,
        shellByShell
    };
    public ReloadType reloadType;  

    public new string name;
    [TextArea(3, 10)]
    public string description;
    public int cost;

    public GameObject weaponObj;
    public GameObject weaponWallBuyObj;
    public GameObject UIObj;
    public HitEffectData hitEffectData;

    public float defaultYPos;
    public float aimingYPos;

    public Sprite UISprite;

    public int magSize;
    public int maxReserveAmmo;
    public float reloadSpeed;
    public int projectileCount;
    public float maxSpreadDeviationAngle;
    public int damage;
    public float headshotMultiplier;
    public float perShotCooldown;
    public float timeToADS;
    public float aimingFOV;
    public AnimationCurve ADSCurve;
    public bool isAutomatic;
    public bool isProjectile;
    public GameObject projectile;
    public float blastRadius;
    public float projectileInitalVelocity;
    public GameObject projectileDestructionEffect;

    public AudioClip[] fireSFX;
    public AudioClip fullReloadSFX, reloadStartSFX, insertShellSFX, reloadEndSFX, equipSFX;

    [Header("Attachments")]
    public bool canUseSightAttachment;
    public bool canUseMagAttachment;
    public bool canUseMuzzleAttachment;
    public bool canUseStockAttachment;
}
