using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager instance;

    
    [SerializeField]
    GameObject crosshairLeftArm, crosshairRightArm, crosshairTopArm, crosshairBottomArm, crosshair, hitMarker;

    [SerializeField]
    float minCrosshairSize, maxCrosshairSize, accuracyRecoveryRate;

    public float currentCrosshairSize, baseCrosshairSize;

    //[SerializeField] AudioSource hitmarkerSFXSource;
    //[SerializeField] AudioClip hitMarkerSfx;
    //Image[] hitmarkerArms;

    //Coroutine deactivateHitMarker;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        currentCrosshairSize = baseCrosshairSize;
    }

    private void OnEnable()
    {
        //ZombieHealth.onDeath += ShowKillHitmarker;
        //ZombieHealth.onHit += ShowNormalHitmarker;
        WeaponShooting.onAimDownSights += UpdateCrosshairVisibility;
        WeaponShooting.onWeaponFired += ExpandCrosshair;
    }

    private void OnDisable()
    {
        //ZombieHealth.onDeath -= ShowKillHitmarker;
        //ZombieHealth.onHit -= ShowNormalHitmarker;
        WeaponShooting.onAimDownSights -= UpdateCrosshairVisibility;
        WeaponShooting.onWeaponFired -= ExpandCrosshair;
    }

    private void Update()
    {
        UpdateCrosshairSize();
    }

    void UpdateCrosshairVisibility(bool isCrosshairVisible, WeaponShooting weaponAiming)
    {
        crosshair.SetActive(isCrosshairVisible);
    }

    void ExpandCrosshair(bool isAiming)
    {
        if (!isAiming)
        {
            crosshairBottomArm.transform.localPosition -= new Vector3 (0, 25, 0);
            crosshairTopArm.transform.localPosition += new Vector3 (0, 25, 0);
            crosshairRightArm.transform.localPosition -= new Vector3 (25, 0, 0);
            crosshairLeftArm.transform.localPosition += new Vector3 (25, 0, 0);
        }
    }

    public void UpdateCrosshairSize()
    {
        if (PlayerMovement.instance.isCrouching)
            currentCrosshairSize = baseCrosshairSize / 2;
        else if (PlayerMovement.instance.currentVelocity.magnitude > 1 && !PlayerMovement.instance.isCrouching && !PlayerMovement.instance.isSprinting)
            currentCrosshairSize = baseCrosshairSize * 1.5f;
        else if (PlayerMovement.instance.isSprinting && !PlayerMovement.instance.isCrouching)
            currentCrosshairSize = baseCrosshairSize * 2;
        else
            currentCrosshairSize = baseCrosshairSize;


        if (crosshairBottomArm.transform.localPosition.magnitude > currentCrosshairSize)
        {
            crosshairBottomArm.transform.localPosition += Vector3.up * Time.deltaTime * accuracyRecoveryRate;
        }
        else if(crosshairBottomArm.transform.localPosition.magnitude < currentCrosshairSize)
        {
            crosshairBottomArm.transform.localPosition -= Vector3.up * Time.deltaTime * accuracyRecoveryRate;
        }

        if (crosshairTopArm.transform.localPosition.magnitude > currentCrosshairSize)
        {
            crosshairTopArm.transform.localPosition += Vector3.down * Time.deltaTime * accuracyRecoveryRate;
        }
        else if (crosshairTopArm.transform.localPosition.magnitude < currentCrosshairSize)
        {
            crosshairTopArm.transform.localPosition -= Vector3.down * Time.deltaTime * accuracyRecoveryRate;
        }

        if (crosshairLeftArm.transform.localPosition.magnitude > currentCrosshairSize)
        {
            crosshairLeftArm.transform.localPosition += Vector3.left * Time.deltaTime * accuracyRecoveryRate;
        }
        else if (crosshairLeftArm.transform.localPosition.magnitude < currentCrosshairSize)
        {
            crosshairLeftArm.transform.localPosition -= Vector3.left * Time.deltaTime * accuracyRecoveryRate;
        }

        if (crosshairRightArm.transform.localPosition.magnitude > currentCrosshairSize)
        {
            crosshairRightArm.transform.localPosition += Vector3.right * Time.deltaTime * accuracyRecoveryRate;
        }
        else if (crosshairRightArm.transform.localPosition.magnitude < currentCrosshairSize)
        {
            crosshairRightArm.transform.localPosition -= Vector3.right * Time.deltaTime * accuracyRecoveryRate;
        }
    }
}

    //void ShowKillHitmarker(int playerIndex, bool wasHeadshot)
    //{
    //    ActivateHitMarker(Color.red);
    //}

    //void ShowNormalHitmarker()
    //{
    //    ActivateHitMarker(Color.white);
    //}

    //public void ActivateHitMarker(Color hitmarkerColour)
    //{
    //    hitMarker.SetActive(true);
    //    SetHitMarkerArmColour(hitmarkerColour);
    //    if(hitmarkerSFXSource.isPlaying) hitmarkerSFXSource.Stop();
    //    //hitmarkerSFXSource.PlayOneShot(hitMarkerSfx);
    //    if (deactivateHitMarker != null)
    //        StopCoroutine(deactivateHitMarker);

    //    deactivateHitMarker = StartCoroutine(DeactivateHitMarker());
    //}

    //void SetHitMarkerArmColour(Color color)
    //{
    //    hitmarkerArms = hitMarker.transform.GetComponentsInChildren<Image>();
    //    for (int i = 0; i < hitmarkerArms.Length; i++)
    //    {
    //        hitmarkerArms[i].color = color;
    //    }
    //}

    //IEnumerator DeactivateHitMarker()
    //{
    //    yield return new WaitForSeconds(.1f);
    //    hitMarker.transform.GetChild(0).gameObject.SetActive(false);
    //}