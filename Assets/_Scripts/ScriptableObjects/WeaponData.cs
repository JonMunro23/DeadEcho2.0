using UnityEngine;

[CreateAssetMenu(fileName = "WeaponObject", menuName = "New Weapon")]
public class WeaponData : ScriptableObject
{
    public enum WeaponSlotType
    {
        primary,
        secondary
    };
    public WeaponSlotType weaponSlotType;

    public enum WeaponType
    {
        Pistol,
        SMG,
        Rifle,
        Shotgun,
        Explosive
    };
    public WeaponType weaponType;

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
    //public float perShotCooldown;
    public float fireRate;
    public float timeToADS;
    public float aimingFOV;
    public AnimationCurve ADSCurve;
    public bool isAutomatic;
    public bool isScoped;
    public Sprite scopeOverlay;
    public bool isProjectile;
    public GameObject projectile;
    public float projectileInitalVelocity;

    public AudioClip[] fireSFX;
    public AudioClip fullReloadSFX, reloadStartSFX, insertShellSFX, reloadEndSFX, equipSFX;

    public float reloadStartAnimLength, reloadShellAnimLength, reloadEndAnimLength;

    [Header("Recoil")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float ADSRecoilX;
    public float ADSRecoilY;
    public float ADSRecoilZ;

    [Header("Attachments")]
    public bool canUseSightAttachment;
    public bool canUseMagAttachment;
    public bool canUseMuzzleAttachment;
    public bool canUseStockAttachment;
}
