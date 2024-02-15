using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Runtime.CompilerServices;

public class WeaponShooting : MonoBehaviour
{
    [Header("Keybinds")]
    KeyCode fireKey = KeyCode.Mouse0;
    KeyCode ADSKey = KeyCode.Mouse1;
    public KeyCode reloadKey = KeyCode.R;

    public bool canShoot;
    public Animator weaponAnimator;

    public Weapon equippedWeapon;
    int maxMagSize, maxReserveAmmo, loadedAmmoBeforeReload, reserveAmmoBeforeReload, damage, projectileCount;
    public int currentReserveAmmo, currentLoadedAmmo;
    float maxSpreadDeviationAngle, perShotCooldown, reloadSpeed, headshotMultiplier;
    bool isAutomatic;
    public bool isAiming, canADS, isReloading;
    public GameObject zombieHitEffect;
    GameObject bulletHole, muzzleEffect;
    Image crosshair;
    AudioSource SFXSource;
    AudioClip[] firingSFX;
    AudioClip reloadSFX; 
    Coroutine reloadCooldownCoRoutine;
    Camera weaponCamera;
    float weaponSpreadDeviation;

    public static event Action<bool, WeaponShooting> onAimDownSights;
    public static event Action<int, int> onAmmoUpdated;

    bool isInstantKillActive = false;

    private void Awake()
    {
        weaponCamera = GameObject.FindGameObjectWithTag("WeaponCamera").GetComponent<Camera>();
        SFXSource = GameObject.FindGameObjectWithTag("WeaponSFX").GetComponent<AudioSource>();
        crosshair = GameObject.FindGameObjectWithTag("HitMarker").GetComponentInParent<Image>();
    }

    private void OnEnable()
    {
        canShoot = true;
        canADS = true;

        PlayerMelee.onMeleePerformed += CancelWeaponActions;
        PlayerThrowables.onEquipmentUsed += CancelWeaponActions;
        InstantKill.onInstantKillGrabbed += EnableInstantKills;
        PowerUpManager.onInstantKillEnded += DisableInstantKills;
    }

    private void OnDisable()
    {
        PlayerMelee.onMeleePerformed -= CancelWeaponActions;
        PlayerThrowables.onEquipmentUsed -= CancelWeaponActions;
        InstantKill.onInstantKillGrabbed -= EnableInstantKills;
        PowerUpManager.onInstantKillEnded -= DisableInstantKills;

    }

    public void InitialiseNewWeaponObj(Weapon weaponToInitialise)
    {
        weaponAnimator = GetComponent<Animator>();
        PlayerMovement.instance.animator = weaponAnimator;
        equippedWeapon = weaponToInitialise;
        maxMagSize = weaponToInitialise.magSize;
        maxReserveAmmo = weaponToInitialise.maxReserveAmmo;
        currentLoadedAmmo = maxMagSize;
        currentReserveAmmo = maxReserveAmmo;
        reloadSpeed = weaponToInitialise.reloadSpeed;
        damage = weaponToInitialise.damage;
        projectileCount = weaponToInitialise.projectileCount;
        perShotCooldown = weaponToInitialise.perShotCooldown;
        maxSpreadDeviationAngle = weaponToInitialise.maxSpreadDeviationAngle;
        isAutomatic = weaponToInitialise.isAutomatic;
        bulletHole = weaponToInitialise.bulletHole;
        muzzleEffect = weaponToInitialise.muzzleEffect;
        firingSFX = weaponToInitialise.fireSFX;
        reloadSFX = weaponToInitialise.reloadSFX;
        headshotMultiplier = weaponToInitialise.headshotMultiplier;
        onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);

        CheckInstantKillStatus();
    }
    public void CheckInstantKillStatus()
    {
        if (PowerUpManager.Instance.GetInstantKillStatus())
        {
            isInstantKillActive = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(fireKey) && isAutomatic)
        {
            FireWeapon();
        }

        if(Input.GetKeyDown(reloadKey) && currentLoadedAmmo != maxMagSize && currentReserveAmmo > 0 && !isReloading)
        {
            ReloadWeapon();
        }

        if (Input.GetKeyDown(ADSKey) && canADS)
        {
            ToggleWeaponAiming();
        }
    }

    void FireWeapon()
    {
        if (canShoot && currentLoadedAmmo != 0 && !isReloading && !PlayerMovement.instance.isSprinting)
        {
            canShoot = false;

            Vector3 forwardVector = Vector3.forward;
            if (PlayerMovement.instance.isCrouching)
            {
                weaponSpreadDeviation = (Random.Range(-maxSpreadDeviationAngle, maxSpreadDeviationAngle) / 2);
            }
            else
            {
                weaponSpreadDeviation = Random.Range(-maxSpreadDeviationAngle, maxSpreadDeviationAngle);
            }
            float angle = Random.Range(-360, 360f);
            forwardVector = Quaternion.AngleAxis(weaponSpreadDeviation, Vector3.up) * forwardVector;
            forwardVector = Quaternion.AngleAxis(angle, Vector3.forward) * forwardVector;

            forwardVector = weaponCamera.transform.rotation * forwardVector;


            SFXSource.PlayOneShot(PickAudioClip());
            if(isAiming)
            {
                weaponAnimator.SetTrigger("AimingFire");
            }
            else if (!isAiming)
            {
                weaponAnimator.SetTrigger("Fire");

                //Simulate recoil

            }
            currentLoadedAmmo--;
            onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.7f, transform.position.z), isAiming == false ? forwardVector : transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
                if(damageable != null)
                {
                    if(isInstantKillActive)
                    {
                        damageable.Kill();
                    }
                    else
                    {
                        if(hit.transform.CompareTag("ZombieHead"))
                        {
                            damageable.OnDamaged(Mathf.RoundToInt(damage * headshotMultiplier), hit.transform.tag);
                        }
                        else
                            damageable.OnDamaged(damage, hit.transform.tag);
                    }

                }
            }

            StartCoroutine(PerShotCooldown());
        }
        else if (canShoot && currentLoadedAmmo == 0 && currentReserveAmmo > 0)
        {
            ReloadWeapon();
        }
    }


    void ToggleWeaponAiming()
    {
        if (isAiming)
        {
            StopADS();
        }
        else if(!isAiming)
        {
            BeginADS();
        }
    }

    void BeginADS()
    {
        onAimDownSights?.Invoke(isAiming, this);
        isAiming = true;

        weaponAnimator.SetBool("isAiming", isAiming);
        crosshair.enabled = false;
    }

    public void StopADS()
    {
        onAimDownSights?.Invoke(isAiming, this);
        isAiming = false;

        weaponAnimator.SetBool("isAiming", isAiming);
        crosshair.enabled = true;
    }

    void ReloadWeapon()
    {
        if (isAiming)
        {
            StopADS();
        }
        canADS = false;
        canShoot = false;
        isReloading = true;
        SFXSource.PlayOneShot(reloadSFX);
        weaponAnimator.SetTrigger("Reload");
        loadedAmmoBeforeReload = currentLoadedAmmo;
        reserveAmmoBeforeReload = currentReserveAmmo;
        reloadCooldownCoRoutine = StartCoroutine(ReloadCooldown());
        if (currentLoadedAmmo != 0)
        {
            int ammoToLoad = maxMagSize - currentLoadedAmmo;
            if (currentReserveAmmo >= ammoToLoad)
            {
                currentLoadedAmmo += ammoToLoad;
                currentReserveAmmo -= ammoToLoad;

            }
            else
            {
                currentLoadedAmmo += currentReserveAmmo;
                currentReserveAmmo = 0;
            }
        }
        else
        {
            if (currentReserveAmmo >= maxMagSize)
            {
                currentLoadedAmmo = maxMagSize;
                currentReserveAmmo -= maxMagSize;
            }
            else
            {
                currentLoadedAmmo += currentReserveAmmo;
                currentReserveAmmo = 0;
            }
        }
    }

    public void CancelReload()
    {
        StopCoroutine(reloadCooldownCoRoutine);
        currentLoadedAmmo = loadedAmmoBeforeReload;
        currentReserveAmmo = reserveAmmoBeforeReload;
        canShoot = true;
        isReloading = false;
    }

    void CancelWeaponActions()
    {
        if (isReloading) CancelReload();
        if (isAiming) StopADS();
    }
    public void RefillAmmo()
    {
        currentLoadedAmmo = maxMagSize;
        currentReserveAmmo = maxReserveAmmo;

        if(WeaponSwapping.instance.currentlyEquippedWeaponObj == gameObject)
            onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
    }

    public bool IsAmmoFull()
    {
        if (currentLoadedAmmo == maxMagSize && currentReserveAmmo == maxReserveAmmo)
        {
            return true;
        }
        else
            return false;
    }

    AudioClip PickAudioClip()
    {
        int rand = Random.Range(0, firingSFX.Length);
        return firingSFX[rand];
    }

    void EnableInstantKills()
    {
        isInstantKillActive = true;
        Debug.Log("instant kill active");
    }

    void DisableInstantKills()
    {
        isInstantKillActive = false;
        Debug.Log("instant kill disabled");
    }

    IEnumerator PerShotCooldown()
    {
        yield return new WaitForSeconds(perShotCooldown);
        canShoot = true;
    }

    IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(reloadSpeed);
        onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
        isReloading = false;
        canShoot = true;
        canADS = true;
    }

}
