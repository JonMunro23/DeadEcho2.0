using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode ADSKey = KeyCode.Mouse1;
    public KeyCode reloadKey = KeyCode.R;

    public bool canShoot;
    public Animator weaponAnimator;

    Weapon equippedWeaponObj;
    int maxMagSize, maxReserveAmmo, loadedAmmoBeforeReload, reserveAmmoBeforeReload, damage, projectileCount;
    public int currentReserveAmmo, currentLoadedAmmo;
    float maxSpreadDeviationAngle, perShotCooldown, reloadSpeed, aimingFOV;
    bool isAutomatic;
    public bool isAiming, canADS, isReloading;
    GameObject bulletHole, muzzleEffect;
    
    AudioSource SFXSource;
    AudioClip[] firingSFX;
    AudioClip reloadSFX;

    Coroutine reloadCooldownCoRoutine;
    Camera weaponCamera;
    float defaultFOV;

    private void Awake()
    {
        weaponCamera = GameObject.FindGameObjectWithTag("WeaponCamera").GetComponent<Camera>();
        SFXSource = GameObject.FindGameObjectWithTag("WeaponSFX").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        canShoot = true;
        defaultFOV = weaponCamera.fieldOfView;
        canADS = true;
    }

    public void InitialiseNewWeaponObj(Weapon weaponToInitialise)
    {
        weaponAnimator = GetComponent<Animator>();
        PlayerMovement.instance.animator = weaponAnimator;
        equippedWeaponObj = weaponToInitialise;
        maxMagSize = weaponToInitialise.magSize;
        maxReserveAmmo = weaponToInitialise.maxReserveAmmo;
        currentLoadedAmmo = maxMagSize;
        currentReserveAmmo = maxReserveAmmo;
        AmmoManager.instance.UpdateAmmoHUD(currentLoadedAmmo, currentReserveAmmo);
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
        aimingFOV = weaponToInitialise.aimingFOV;
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
        if (canShoot && currentLoadedAmmo != 0)
        {
            canShoot = false;
            SFXSource.PlayOneShot(PickAudioClip());
            if(isAiming)
            {
                weaponAnimator.SetTrigger("AimingFire");
            }
            else if (!isAiming)
            {
                weaponAnimator.SetTrigger("Fire");
            }
            currentLoadedAmmo--;
            AmmoManager.instance.UpdateAmmoHUD(currentLoadedAmmo, currentReserveAmmo);
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.7f, transform.position.z), transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                Instantiate(bulletHole, hit.point, Quaternion.identity);
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
        isAiming = true;
        PlayerMovement.instance.BeginAiming();
        weaponCamera.fieldOfView = aimingFOV;
        weaponAnimator.SetBool("isAiming", isAiming);
        WeaponSway.isWeaponSwayActive = false;
    }

    public void StopADS()
    {
        isAiming = false;
        PlayerMovement.instance.StopAiming();
        weaponCamera.fieldOfView = defaultFOV;
        weaponAnimator.SetBool("isAiming", isAiming);
        WeaponSway.isWeaponSwayActive = true;
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
        Debug.Log("Reload Cancelled");
        StopCoroutine(reloadCooldownCoRoutine);
        Debug.Log("Stopped: "+ reloadCooldownCoRoutine);
        currentLoadedAmmo = loadedAmmoBeforeReload;
        currentReserveAmmo = reserveAmmoBeforeReload;
        canShoot = true;
        isReloading = false;
    }


    AudioClip PickAudioClip()
    {
        int rand = Random.Range(0, firingSFX.Length);
        return firingSFX[rand];
    }

    IEnumerator PerShotCooldown()
    {
        yield return new WaitForSeconds(perShotCooldown);
        canShoot = true;
    }

    IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(reloadSpeed);
        AmmoManager.instance.UpdateAmmoHUD(currentLoadedAmmo, currentReserveAmmo);
        isReloading = false;
        canShoot = true;
        canADS = true;
    }
}
