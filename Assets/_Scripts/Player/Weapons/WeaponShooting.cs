using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

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
    bool isAutomatic, isPlayerDead;
    public bool isAiming, canADS, isReloading, canReload;
    public float fireRate;

    [SerializeField]
    GameObject bulletHole;
    Image scopeOverlay, scopeOverlayFade;
    [SerializeField]
    ParticleSystem muzzleEffect;
    //Image crosshair;
    AudioSource SFXSource;
    AudioClip[] firingSFX;
    AudioClip fullReloadSFX, insertShellSFX, reloadStartSFX, reloadEndSFX; 
    Coroutine reloadCooldownCoroutine, shellByShellReloadCoroutine;
    Camera weaponCamera;
    float weaponSpreadDeviation;
    SkinnedMeshRenderer[] weaponSkinnedMeshRenderers;


    public static event Action<bool, WeaponShooting> onAimDownSights;
    public static event Action<int, int> onAmmoUpdated;
    public static event Action<bool> onWeaponFired;

    bool isInstantKillActive, isBottomlessClipActive;
    bool stopShellByShellReload;

    public bool IsADSToggle;
    private void Awake()
    {
        weaponCamera = GameObject.FindGameObjectWithTag("WeaponCamera").GetComponent<Camera>();
        SFXSource = GameObject.FindGameObjectWithTag("WeaponSFX").GetComponent<AudioSource>();
        scopeOverlay = GameObject.FindGameObjectWithTag("ScopeOverlay").GetComponent<Image>();
        weaponSkinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
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
        InstantKill.onInstantKillGrabbed += EnableInstantKills;
        BottomlessClip.onBottomlessClipGrabbed += EnableBottomlessClip;
        PowerUpManager.onInstantKillEnded += DisableInstantKills;
        PowerUpManager.onBottomlessClipEnded += DisableBottomlessClip;
        //PlayerStats.onUpgradeApplied += ApplyUpgradeModifiers;
    }

    private void OnDisable()
    {
        PlayerMelee.onMeleePerformed -= CancelWeaponActions;
        PlayerHealing.onSyringeUsed -= CancelWeaponActions;
        PlayerHealth.onDeath -= OnPlayerDeath;
        PlayerThrowables.onEquipmentUsed -= CancelWeaponActions;
        InstantKill.onInstantKillGrabbed -= EnableInstantKills;
        BottomlessClip.onBottomlessClipGrabbed -= EnableBottomlessClip;
        PowerUpManager.onInstantKillEnded -= DisableInstantKills;
        PowerUpManager.onBottomlessClipEnded -= DisableBottomlessClip;
        //PlayerStats.onUpgradeApplied -= ApplyUpgradeModifiers;
    }

    public void InitialiseWeapon(Weapon weaponToInitialise)
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
        fireRate = weaponToInitialise.fireRate;
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

        if (weaponToInitialise.isScoped == true)
            scopeOverlayFade = scopeOverlay.transform.GetChild(0).GetComponent<Image>();

        headshotMultiplier = weaponToInitialise.headshotMultiplier;
        onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
        CheckInstantKillStatus();
        CheckBottomlessClipStatus();

        //ApplyUpgradeModifiers();
    }

    //public void ApplyUpgradeModifiers()
    //{
    //    fireRate += fireRate * PlayerStats.fireRateModifier;
    //    damage += Mathf.RoundToInt(damage * PlayerStats.damageModifier);

    //    reloadSpeedMultiplier += PlayerStats.reloadSpeedModifier;
    //    weaponAnimator.SetFloat("reloadSpeedMultiplier", reloadSpeedMultiplier);
    //    reloadSpeed -= reloadSpeed * PlayerStats.reloadSpeedModifier;

    //    Debug.Log("upgrades applied");
    //}

    public void CheckInstantKillStatus()
    {
        if (PowerUpManager.Instance.GetInstantKillStatus())
        {
            isInstantKillActive = true;
        }
        else
            isInstantKillActive = false;
    }

    public void CheckBottomlessClipStatus()
    {
        if(PowerUpManager.Instance.GetBottomlessClipStatus())
        {
            isBottomlessClipActive = true;
        }
        else
            isBottomlessClipActive = false;
    }

    void OnPlayerDeath()
    {
        isPlayerDead = true;
        canShoot = false;
        canADS = false;
        canReload = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!PauseMenu.isPaused && !UpgradeSelectionMenu.isUpgradeSelectionMenuOpen)
        {
            if(isAutomatic)
            {
                if(Input.GetKey(fireKey))
                {
                    if (equippedWeapon.reloadType == Weapon.ReloadType.shellByShell && isReloading == true)
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
                    if (equippedWeapon.reloadType == Weapon.ReloadType.shellByShell && isReloading == true)
                    {
                        InterruptShellByShellReload();
                        return;
                    }

                    FireWeapon();
                }
            }

            

            if(Input.GetKeyDown(reloadKey) && currentLoadedAmmo < maxMagSize && currentReserveAmmo > 0 && !isReloading)
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
        if (canShoot && (currentLoadedAmmo != 0 || isBottomlessClipActive) && !isReloading)
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
            }

            if(!isBottomlessClipActive)
                currentLoadedAmmo--;

            onWeaponFired?.Invoke(isAiming);
            onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);

            for (int i = 0; i < projectileCount; i++)
            {
                if (equippedWeapon.isProjectile)
                {
                    //fire projectile
                    GameObject clone = Instantiate(equippedWeapon.projectile, muzzleEffect.transform.position, transform.rotation);
                    clone.GetComponent<Rigidbody>().AddForce(transform.forward * equippedWeapon.projectileInitalVelocity, ForceMode.Impulse);
                }
                else //fire hitscan
                {
                    Vector3 forwardVector = Vector3.forward;
                    if (PlayerMovement.instance.isCrouching)
                    {
                        weaponSpreadDeviation = (Random.Range(-maxSpreadDeviationAngle, maxSpreadDeviationAngle) / 2);
                    }
                    else if(PlayerMovement.instance.currentVelocity.magnitude > 0 && !PlayerMovement.instance.isCrouching)
                    {
                        weaponSpreadDeviation = (Random.Range(-maxSpreadDeviationAngle, maxSpreadDeviationAngle) * 2);
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
                        if (damageable != null)
                        {
                            if (isInstantKillActive)
                            {
                                damageable.InstantlyKill();
                            }
                            else
                            {
                                if (hit.transform.CompareTag("ZombieHead"))
                                {
                                    damageable.OnDamaged(Mathf.RoundToInt((damage + (damage * PlayerStats.damageModifier)) * headshotMultiplier), true);
                                }
                                else
                                {
                                    damageable.OnDamaged(Mathf.RoundToInt(damage + (damage * PlayerStats.damageModifier)), false);
                                }

                            }
                        }

                        if (hit.transform.TryGetComponent<SurfaceIdentifier>(out SurfaceIdentifier _surface))
                        {
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
                    }
                }
            }
        }
        else if (canShoot && (currentLoadedAmmo == 0 && !isBottomlessClipActive) && currentReserveAmmo > 0)
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
        if(isAiming == false)
        {
            onAimDownSights?.Invoke(isAiming, this);
            isAiming = true;
            weaponAnimator.SetBool("isAiming", isAiming);
            if(equippedWeapon.isScoped)
            {
                scopeOverlayFade.DOColor(Color.black, .4f).SetDelay(.2f).OnComplete(() =>
                {
                    scopeOverlay.enabled = true;
                    scopeOverlay.sprite = equippedWeapon.scopeOverlay;
                    HideWeaponModel();
                    scopeOverlayFade.DOColor(Color.clear, .2f);
                });
            }
        }
    }

    public void StopADS()
    {
        if(isAiming)
        {
            onAimDownSights?.Invoke(isAiming, this);
            isAiming = false;
            weaponAnimator.SetBool("isAiming", isAiming);
            if (equippedWeapon.isScoped)
            {
                scopeOverlayFade.DOColor(Color.black, .2f).OnComplete(() =>
                {
                    scopeOverlay.enabled = false;
                    scopeOverlay.sprite = null;
                    ShowWeaponModel();
                    scopeOverlayFade.DOColor(Color.clear, .1f);
                });
            }
        }
    }

    void ShowWeaponModel()
    {
        foreach (SkinnedMeshRenderer meshRenderer in weaponSkinnedMeshRenderers) { meshRenderer.enabled = true; }
    }

    void HideWeaponModel()
    {
        foreach (SkinnedMeshRenderer meshRenderer in weaponSkinnedMeshRenderers) { meshRenderer.enabled = false; }
    }

    void ReloadWeapon()
    {
        if(canReload)
        {
            weaponAnimator.SetFloat("reloadSpeedMultiplier", 1 + PlayerStats.reloadSpeedModifier);
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
            else if (equippedWeapon.reloadType == Weapon.ReloadType.shellByShell)
            {
                shellByShellReloadCoroutine = StartCoroutine(ShellByShellReload());
            }
        }
    }

    IEnumerator ShellByShellReload()
    {
        weaponAnimator.Play("StartReload");
        loadedAmmoBeforeReload = currentLoadedAmmo;
        reserveAmmoBeforeReload = currentReserveAmmo;
        //SFXSource.PlayOneShot(reloadStartSFX);
        yield return new WaitForSeconds(equippedWeapon.reloadStartAnimLength - (equippedWeapon.reloadStartAnimLength * PlayerStats.reloadSpeedModifier));
        //maxmagsize - 1 due to EndReload anim slotting a round
        while (currentLoadedAmmo < maxMagSize - 1 && !stopShellByShellReload)
        {
            weaponAnimator.Play("InsertShell");
            
            loadedAmmoBeforeReload = currentLoadedAmmo;
            reserveAmmoBeforeReload = currentReserveAmmo;
            yield return new WaitForSeconds(equippedWeapon.reloadShellAnimLength - (equippedWeapon.reloadShellAnimLength * PlayerStats.reloadSpeedModifier));
            currentLoadedAmmo++;
            currentReserveAmmo--;
            onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);

        }

        if (stopShellByShellReload)
            stopShellByShellReload = false;

        weaponAnimator.Play("EndReload");
        loadedAmmoBeforeReload = currentLoadedAmmo;
        reserveAmmoBeforeReload = currentReserveAmmo;
        yield return new WaitForSeconds(equippedWeapon.reloadEndAnimLength - (equippedWeapon.reloadEndAnimLength * PlayerStats.reloadSpeedModifier));
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
        weaponAnimator.Rebind();
        weaponAnimator.Update(0f);

        SFXSource.Stop();

        if (equippedWeapon.reloadType == Weapon.ReloadType.shellByShell)
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
        currentLoadedAmmo = maxMagSize;
        currentReserveAmmo = maxReserveAmmo;
        loadedAmmoBeforeReload = currentLoadedAmmo;
        reserveAmmoBeforeReload = currentReserveAmmo;

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

    void EnableBottomlessClip()
    {
        isBottomlessClipActive = true;
    }

    void DisableBottomlessClip()
    {
        isBottomlessClipActive = false;
    }

    IEnumerator PerShotCooldown()
    {
        float modifiedFireRate = fireRate + (fireRate * PlayerStats.fireRateModifier);
        float perSecCooldown = 1 / (modifiedFireRate / 60f);
        yield return new WaitForSeconds(perSecCooldown);
        if (!isPlayerDead)
            canShoot = true;
    }

    //IEnumerator ReloadCooldown()
    //{
    //    Debug.Log(reloadSpeed);
    //    Debug.Log(reloadSpeed - (reloadSpeed * PlayerStats.reloadSpeedModifier));
    //    yield return new WaitForSeconds(reloadSpeed - (reloadSpeed * PlayerStats.reloadSpeedModifier));
    //    onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
    //    isReloading = false;
    //    canShoot = true;
    //    canADS = true;
    //}

    //Called from animation event in reload anim
    public void ReloadFinished()
    {
        onAmmoUpdated?.Invoke(currentLoadedAmmo, currentReserveAmmo);
        isReloading = false;
        canShoot = true;
        canADS = true;
    }

}
