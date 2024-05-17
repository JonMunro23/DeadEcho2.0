using System.Collections;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponShooting : MonoBehaviour
{
    [Header("Keybinds")]
    KeyCode fireKey = KeyCode.Mouse0;
    KeyCode ADSKey = KeyCode.Mouse1;
    public KeyCode reloadKey = KeyCode.R;

    [Header("Data")]
    [SerializeField] HitEffectData hitEffectData;
    public WeaponData weaponData;
    public int weaponSlot;
    public bool canShoot;
    //public Animator weaponAnimator;
    public Animator armsAnimator;
    int maxMagSize, maxReserveAmmo, loadedAmmoBeforeReload, reserveAmmoBeforeReload, weaponBaseDamage, projectileCount;
    public int currentReserveAmmo, currentLoadedAmmo;
    float maxSpreadDeviationAngle, reloadSpeed, headshotMultiplier;
    bool isAutomatic, isPlayerDead;
    public bool isAiming, canADS, isReloading, canReload;
    public float fireRate;

    [SerializeField]
    GameObject bulletHole;
    [SerializeField]
    ParticleSystem[] muzzleEffects;
    //Image crosshair;
    AudioSource SFXSource;
    AudioClip[] firingSFX;
    AudioClip fullReloadSFX, insertShellSFX, reloadStartSFX, reloadEndSFX; 
    Coroutine reloadCooldownCoroutine, shellByShellReloadCoroutine;
    //float weaponSpreadDeviation;


    public static event Action<bool, WeaponShooting> onAimDownSights;
    public static event Action<int, int> onAmmoUpdated;
    public static event Action<bool> onWeaponFired;

    bool stopShellByShellReload;

    public bool IsADSToggle;

    public Transform weaponLeftHandTarget, weaponRightHandTarget;

    [Header("Laser Pointers")]
    [SerializeField]
    LaserPointer[] laserPointers;

    private void Awake()
    {
        SFXSource = GameObject.FindGameObjectWithTag("WeaponSFX").GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        canShoot = true;
        canADS = true;
        canReload = true;

        PlayerMelee.onMeleePerformed += CancelWeaponActions;
        PlayerHealing.onSyringeUsed += CancelWeaponActions;
        PlayerHealth.onDeath += OnPlayerDeath;
        PlayerThrowables.onEquipmentUsed += CancelWeaponActions;

    }



    private void OnDisable()
    {
        PlayerMelee.onMeleePerformed -= CancelWeaponActions;
        PlayerHealing.onSyringeUsed -= CancelWeaponActions;
        PlayerHealth.onDeath -= OnPlayerDeath;
        PlayerThrowables.onEquipmentUsed -= CancelWeaponActions;

        DeinitLaserPointers();
    }

    public void InitialiseWeapon(WeaponData weaponToInitialise, Animator FPSArmsAnimator)
    {
        armsAnimator = FPSArmsAnimator;
        weaponData = weaponToInitialise;
        maxMagSize = weaponToInitialise.magSize;
        maxReserveAmmo = weaponToInitialise.maxReserveAmmo;
        currentLoadedAmmo = maxMagSize;
        currentReserveAmmo = maxReserveAmmo;
        reloadSpeed = weaponToInitialise.reloadSpeed;
        weaponBaseDamage = weaponToInitialise.damage;
        projectileCount = weaponToInitialise.projectileCount;
        fireRate = weaponToInitialise.fireRate;
        maxSpreadDeviationAngle = weaponToInitialise.maxSpreadDeviationAngle;
        isAutomatic = weaponToInitialise.isAutomatic;
        hitEffectData = weaponToInitialise.hitEffectData;
        firingSFX = weaponToInitialise.fireSFX;

        if(weaponToInitialise.reloadType == WeaponData.ReloadType.shellByShell)
        {
            reloadStartSFX = weaponToInitialise.reloadStartSFX;
            insertShellSFX = weaponToInitialise.insertShellSFX;
            reloadEndSFX = weaponToInitialise.reloadEndSFX;
        }
        else
        fullReloadSFX = weaponToInitialise.fullReloadSFX;

        headshotMultiplier = weaponToInitialise.headshotMultiplier;

        if(!weaponData.infiniteAmmo)
            onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);

        //OnWeaponEquipped();
    }

    public void OnWeaponEquipped()
    {
        InitLaserPointers();
    }

    void OnPlayerDeath()
    {
        isPlayerDead = true;
        canShoot = false;
        canADS = false;
        canReload = false;
        DeinitLaserPointers();
    }

    private void InitLaserPointers()
    {
        if (weaponData.hasLaserSight)
        {
            for (int i = 0; i < laserPointers.Length; i++)
            {
                laserPointers[i].SpawnLaser(muzzleEffects[i].transform);
            }
        }
    }

    private void DeinitLaserPointers()
    {
        if (weaponData.hasLaserSight)
        {
            for (int i = 0; i < laserPointers.Length; i++)
            {
                laserPointers[i].RemoveLaser();
            }
        }
    }

    void Update()
    {
        if(!PauseMenu.isPaused && !UpgradeSelectionMenu.isUpgradeSelectionMenuOpen)
        {
            if (isAutomatic)
            {
                if(Input.GetKey(fireKey))
                {
                    if (weaponData.reloadType == WeaponData.ReloadType.shellByShell && isReloading == true)
                    {
                        InterruptShellByShellReload();
                        return;
                    }

                    FireWeapon();
                }
            }
            else if (!isAutomatic)
            {
                if(Input.GetKeyDown(fireKey))
                {
                    if (weaponData.reloadType == WeaponData.ReloadType.shellByShell && isReloading == true)
                    {
                        InterruptShellByShellReload();
                        return;
                    }

                    FireWeapon();
                }
            }

            

            if(!weaponData.infiniteAmmo && Input.GetKeyDown(reloadKey) && currentLoadedAmmo < maxMagSize && currentReserveAmmo > 0 && !isReloading)
            {
                ReloadWeapon();
            }

            if(IsADSToggle)
            {
                if (Input.GetKeyDown(ADSKey) && canADS)
                {
                    ToggleWeaponAiming();
                }
            }
            else
            {
                if (Input.GetKeyDown(ADSKey) && canADS)
                {
                    BeginADS();
                }
                if(Input.GetKeyUp(ADSKey))
                {
                    StopADS();
                }
            }
        }
    }

    void FireWeapon()
    {
        if (canShoot && (currentLoadedAmmo != 0 || PowerUpManager.isBottomlessClipActive) && !isReloading)
        {
            canShoot = false;
            StartCoroutine(PerShotCooldown());
            PlayMuzzleFlash();
            SFXSource.PlayOneShot(PickAudioClip());

            var stateID = Animator.StringToHash("Fire");
            if(armsAnimator.HasState(0, stateID))
            {
                armsAnimator.Play("Fire");
            }

            if(!PowerUpManager.isBottomlessClipActive && !weaponData.infiniteAmmo)
                currentLoadedAmmo--;

            for (int i = 0; i < projectileCount; i++)
            {
                if (weaponData.isProjectile)
                {
                    //fire projectile
                    GameObject clone = Instantiate(weaponData.projectile, muzzleEffects[0].transform.position, transform.rotation);
                    clone.GetComponent<Rigidbody>().AddForce(transform.forward * weaponData.projectileInitalVelocity, ForceMode.Impulse);
                }
                else //fire hitscan
                {
                    Vector3 forwardVector = muzzleEffects[0].transform.forward;
                    //if (PlayerMovement.instance.isCrouching)
                    //{
                    //    weaponSpreadDeviation = (Random.Range(-maxSpreadDeviationAngle, maxSpreadDeviationAngle) / 2);
                    //}
                    //else if(PlayerMovement.instance.currentVelocity.magnitude > 0 && !PlayerMovement.instance.isCrouching)
                    //{
                    //    weaponSpreadDeviation = (Random.Range(-maxSpreadDeviationAngle, maxSpreadDeviationAngle) * 2);
                    //}
                    //else 
                    //{
                    //    weaponSpreadDeviation = Random.Range(-maxSpreadDeviationAngle, maxSpreadDeviationAngle);
                    //}
                    //float angle = Random.Range(-360f, 360f);
                    //forwardVector = Quaternion.AngleAxis(weaponSpreadDeviation, muzzleEffect.transform.up) * forwardVector;
                    //forwardVector = Quaternion.AngleAxis(angle, muzzleEffect.transform.forward) * forwardVector;

                    //forwardVector = weaponCamera.transform.rotation * forwardVector;
                    RaycastHit hit;
                    if (Physics.Raycast(muzzleEffects[0].transform.position, /*isAiming == false ? */forwardVector/* : muzzleEffect.transform.forward*/, out hit, Mathf.Infinity))
                    {
                        IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
                        if (damageable != null)
                        {
                            if (PowerUpManager.isInstantKillActive)
                            {
                                damageable.InstantlyKill();
                            }
                            else
                            {
                                if (hit.transform.CompareTag("ZombieHead"))
                                {
                                    float headshotDamage = (weaponBaseDamage + (weaponBaseDamage * PlayerUpgrades.weaponDamageModifier) * (headshotMultiplier + PlayerUpgrades.bonusheadshotMultiplier));
                                    damageable.OnDamaged(Mathf.RoundToInt(headshotDamage), hit.transform.tag);
                                }
                                else
                                {
                                    damageable.OnDamaged(Mathf.RoundToInt(weaponBaseDamage + (weaponBaseDamage * PlayerUpgrades.weaponDamageModifier)), hit.transform.tag);
                                }

                            }
                        }

                        if (hit.transform.TryGetComponent<SurfaceIdentifier>(out SurfaceIdentifier _surface))
                        {
                            if(_surface.surfaceType == SurfaceTypes.glass)
                            {
                                Destroy(hit.transform.gameObject);
                                return;
                            }

                            Vector3 spawnLocation = hit.point + (hit.normal * .01f);
                            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                            if (_surface.surfaceType != SurfaceTypes.flesh)
                            {
                                GameObject clone = Instantiate(bulletHole, spawnLocation, spawnRotation);
                                clone.transform.SetParent(hit.transform);
                                clone.GetComponent<BulletHole>().SetMaterialType(_surface.surfaceType);
                            }

                            spawnRotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                            hitEffectData.SpawnHitEffect(_surface.surfaceType, spawnLocation, spawnRotation);
                        }

                        if(weaponData.bulletPenData && weaponData.bulletPenData.MaxObjectsToPenetrate > 0)
                        {
                            //penetrate zombies
                        }
                    }
                }
            }

            onWeaponFired?.Invoke(isAiming);
            if(!weaponData.infiniteAmmo)
                onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
        }
        else if (!weaponData.infiniteAmmo && canShoot && (currentLoadedAmmo == 0 && !PowerUpManager.isBottomlessClipActive) && currentReserveAmmo > 0)
        {
            ReloadWeapon();
        }
    }

    void PlayMuzzleFlash()
    {
        muzzleEffects[0].Play();
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
        if(isAiming == false)
        {
            onAimDownSights?.Invoke(isAiming, this);
            isAiming = true;
            //weaponAnimator.SetBool("isAiming", isAiming);
        }
    }

    public void StopADS()
    {
        if(isAiming)
        {
            onAimDownSights?.Invoke(isAiming, this);
            isAiming = false;
            //weaponAnimator.SetBool("isAiming", isAiming);
        }
    }

    void ReloadWeapon()
    {
        if(canReload)
        {
            //weaponAnimator.SetFloat("reloadSpeedMultiplier", 1 + PlayerUpgrades.reloadSpeedModifier);
            if (isAiming)
            {
                StopADS();
            }
            canADS = false;
            canShoot = false;
            isReloading = true;
            if(weaponData.reloadType == WeaponData.ReloadType.magazine)
            {
                SFXSource.PlayOneShot(fullReloadSFX);
                armsAnimator.Play("Reload");
            
                loadedAmmoBeforeReload = currentLoadedAmmo;
                reserveAmmoBeforeReload = currentReserveAmmo;
                //reloadCooldownCoroutine = StartCoroutine(ReloadCooldown());
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
            else if (weaponData.reloadType == WeaponData.ReloadType.shellByShell)
            {
                shellByShellReloadCoroutine = StartCoroutine(ShellByShellReload());
            }
        }
    }

    IEnumerator ShellByShellReload()
    {
        //weaponAnimator.Play("StartReload");
        loadedAmmoBeforeReload = currentLoadedAmmo;
        reserveAmmoBeforeReload = currentReserveAmmo;
        //SFXSource.PlayOneShot(reloadStartSFX);
        yield return new WaitForSeconds(weaponData.reloadStartAnimLength - (weaponData.reloadStartAnimLength * PlayerUpgrades.reloadSpeedModifier));
        //maxmagsize - 1 due to EndReload anim slotting a round
        while (currentLoadedAmmo < maxMagSize - 1 && !stopShellByShellReload)
        {
            //weaponAnimator.Play("InsertShell");
            
            loadedAmmoBeforeReload = currentLoadedAmmo;
            reserveAmmoBeforeReload = currentReserveAmmo;
            yield return new WaitForSeconds(weaponData.reloadShellAnimLength - (weaponData.reloadShellAnimLength * PlayerUpgrades.reloadSpeedModifier));
            currentLoadedAmmo++;
            currentReserveAmmo--;
            onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);

        }

        if (stopShellByShellReload)
            stopShellByShellReload = false;

        //weaponAnimator.Play("EndReload");
        loadedAmmoBeforeReload = currentLoadedAmmo;
        reserveAmmoBeforeReload = currentReserveAmmo;
        yield return new WaitForSeconds(weaponData.reloadEndAnimLength - (weaponData.reloadEndAnimLength * PlayerUpgrades.reloadSpeedModifier));
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

    public void InterruptShellByShellReload()
    {
        stopShellByShellReload = true;
    }

    public void CancelReload()
    {
        //stop weapons remaining in weird positions after cancelling reload
        //weaponAnimator.Rebind();
        //weaponAnimator.Update(0f);

        SFXSource.Stop();

        if (weaponData.reloadType == WeaponData.ReloadType.shellByShell)
        {
            if (shellByShellReloadCoroutine != null)
                StopCoroutine(shellByShellReloadCoroutine);
        }
        else
        {
            if(reloadCooldownCoroutine != null)
                StopCoroutine(reloadCooldownCoroutine);
        }


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
        if(!weaponData.infiniteAmmo)
        {
            currentLoadedAmmo = maxMagSize;
            currentReserveAmmo = maxReserveAmmo;
            loadedAmmoBeforeReload = currentLoadedAmmo;
            reserveAmmoBeforeReload = currentReserveAmmo;

            if(WeaponSwapping.instance.currentlyEquippedWeaponObj == gameObject)
                onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
        }
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

    IEnumerator PerShotCooldown()
    {
        float modifiedFireRate = fireRate + (fireRate * PlayerUpgrades.fireRateModifier);
        float perSecCooldown = 1 / (modifiedFireRate / 60f);
        yield return new WaitForSeconds(perSecCooldown);
        if (!isPlayerDead)
            canShoot = true;
    }

    //Called from animation event in reload anim
    public void ReloadFinished()
    {
        //transform.rotation = Quaternion.Euler(Vector3.zero);
        
        onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
        isReloading = false;
        canShoot = true;
        canADS = true;
    }

}
