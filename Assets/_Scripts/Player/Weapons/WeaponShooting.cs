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

    [Header("Data")]
    [SerializeField] HitEffectData hitEffectData;

    public bool canShoot;
    public Animator weaponAnimator;

    public Weapon equippedWeapon;
    int maxMagSize, maxReserveAmmo, loadedAmmoBeforeReload, reserveAmmoBeforeReload, damage, projectileCount;
    public int currentReserveAmmo, currentLoadedAmmo;
    float maxSpreadDeviationAngle, perShotCooldown, reloadSpeed, headshotMultiplier;
    bool isAutomatic;
    public bool isAiming, canADS, isReloading;
    public GameObject zombieHitEffect;
    [SerializeField]
    GameObject bulletHole;
    [SerializeField]
    ParticleSystem muzzleEffect;
    Image crosshair;
    AudioSource SFXSource;
    AudioClip[] firingSFX;
    AudioClip fullReloadSFX, insertShellSFX, reloadStartSFX, reloadEndSFX; 
    Coroutine reloadCooldownCoRoutine;
    Camera weaponCamera;
    float weaponSpreadDeviation;

    public static event Action<bool, WeaponShooting> onAimDownSights;
    public static event Action<int, int> onAmmoUpdated;

    bool isInstantKillActive = false;
    bool stopShellByShellReload = false;

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
        hitEffectData = weaponToInitialise.hitEffectData;
        firingSFX = weaponToInitialise.fireSFX;

        if(weaponToInitialise.reloadType == Weapon.ReloadType.shellByShell)
        {
            reloadStartSFX = weaponToInitialise.reloadStartSFX;
            insertShellSFX = weaponToInitialise.insertShellSFX;
            reloadEndSFX = weaponToInitialise.reloadEndSFX;
        }
        else
        fullReloadSFX = weaponToInitialise.fullReloadSFX;

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
        if(!PauseMenu.isPaused)
        {
            if(isAutomatic)
            {
                if(Input.GetKey(fireKey))
                {
                    if (equippedWeapon.reloadType == Weapon.ReloadType.shellByShell && isReloading == true)
                    {
                        CancelShellByShellReload();
                        return;
                    }

                    FireWeapon();
                }
            }
            else if (!isAutomatic)
            {
                if(Input.GetKeyDown(fireKey))
                {
                    if (equippedWeapon.reloadType == Weapon.ReloadType.shellByShell && isReloading == true)
                    {
                        CancelShellByShellReload();
                        return;
                    }

                    FireWeapon();
                }
            }

            

            if(Input.GetKeyDown(reloadKey) && currentLoadedAmmo < maxMagSize && currentReserveAmmo > 0 && !isReloading)
            {
                ReloadWeapon();
            }

            if (Input.GetKeyDown(ADSKey) && canADS)
            {
                ToggleWeaponAiming();
            }
        }
    }

    void FireWeapon()
    {
        if (canShoot && currentLoadedAmmo != 0 && !isReloading && !PlayerMovement.instance.isSprinting)
        {
            canShoot = false;
            StartCoroutine(PerShotCooldown());
            PlayMuzzleFlash();
            SFXSource.PlayOneShot(PickAudioClip());
            if(isAiming)
            {
                weaponAnimator.Play("AimingFire");
            }
            else if (!isAiming)
            {
                weaponAnimator.Play("Fire");

                //Simulate recoil

            }
            currentLoadedAmmo--;
            onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 forwardVector = Vector3.forward;
                if (PlayerMovement.instance.isCrouching)
                {
                    weaponSpreadDeviation = (Random.Range(-maxSpreadDeviationAngle, maxSpreadDeviationAngle) / 2);
                }
                else
                {
                    weaponSpreadDeviation = Random.Range(-maxSpreadDeviationAngle, maxSpreadDeviationAngle);
                }
                float angle = Random.Range(-360f, 360f);
                forwardVector = Quaternion.AngleAxis(weaponSpreadDeviation, Vector3.up) * forwardVector;
                forwardVector = Quaternion.AngleAxis(angle, Vector3.forward) * forwardVector;

                forwardVector = weaponCamera.transform.rotation * forwardVector;
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.7f, transform.position.z), isAiming == false ? forwardVector : transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
                {
                    IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
                    if(damageable != null)
                    {
                        if(isInstantKillActive)
                        {
                            damageable.InstantlyKill();
                        }
                        else
                        {
                            if(hit.transform.CompareTag("ZombieHead"))
                            {
                                damageable.OnDamaged(Mathf.RoundToInt(damage * headshotMultiplier), true);
                            }
                            else
                            {
                                damageable.OnDamaged(damage, false);
                            }

                        }
                    }
                
                    if (hit.transform.TryGetComponent<SurfaceIdentifier>(out SurfaceIdentifier _surface))
                    {
                        Vector3 spawnLocation = hit.point + (hit.normal * .01f);
                        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                        if(_surface.surfaceType != SurfaceTypes.flesh)
                        {
                            GameObject clone = Instantiate(bulletHole, spawnLocation, spawnRotation);
                            clone.transform.SetParent(hit.transform);
                            clone.GetComponent<BulletHole>().SetMaterialType(_surface.surfaceType);
                        }

                        spawnRotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                        hitEffectData.SpawnHitEffect(_surface.surfaceType, spawnLocation, spawnRotation);
                    }               
                }

            }

        }
        else if (canShoot && currentLoadedAmmo == 0 && currentReserveAmmo > 0)
        {
            ReloadWeapon();
        }
    }

    void PlayMuzzleFlash()
    {
        muzzleEffect.Play();
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
        if(equippedWeapon.reloadType == Weapon.ReloadType.magazine)
        {
            SFXSource.PlayOneShot(fullReloadSFX);
            weaponAnimator.Play("Reload");
            
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
        else if (equippedWeapon.reloadType == Weapon.ReloadType.shellByShell)
        {
            StartCoroutine(ShellByShellReload());
        }
    }

    IEnumerator ShellByShellReload()
    {
        weaponAnimator.Play("StartReload");
        //SFXSource.PlayOneShot(reloadStartSFX);
        yield return new WaitForSeconds(.6f);

        //maxmagsize - 1 due to EndReload anim slotting a round
        while (currentLoadedAmmo < maxMagSize - 1 && !stopShellByShellReload)
        {
            weaponAnimator.Play("InsertShell");
            yield return new WaitForSeconds(1.4f);
            currentLoadedAmmo++;
            currentReserveAmmo--;
            onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);

        }
        if (stopShellByShellReload)
            stopShellByShellReload = false;

        weaponAnimator.Play("EndReload");
        yield return new WaitForSeconds(2.4f);
        currentLoadedAmmo++;
        currentReserveAmmo--;
        onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
        isReloading = false;
        canShoot = true;
        canADS = true;
    }

    //used for animation events on shotgun
    void PlayEndReloadSFX()
    {
        SFXSource.PlayOneShot(reloadEndSFX);
    }

    //used for animation events on shotgun
    void PlayShellInsertionSFX()
    {
        SFXSource.PlayOneShot(insertShellSFX);
    }

    public void CancelShellByShellReload()
    {
        stopShellByShellReload = true;
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
    }

    void DisableInstantKills()
    {
        isInstantKillActive = false;
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
