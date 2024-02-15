using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAnimationEvents : MonoBehaviour
{
    AudioSource meleeSFXSource;
    [SerializeField] AudioClip swingWeaponSFX, swingingWeaponEffortSFX, hitEnemySFX;

    [SerializeField]
    PlayerMelee playerMelee;

    private void Awake()
    {
        meleeSFXSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySlashSFX()
    {
        meleeSFXSource.PlayOneShot(swingWeaponSFX);
    }

    public void PlayEffortSFX()
    {
        meleeSFXSource.PlayOneShot(swingingWeaponEffortSFX);
    }

    public void CheckForHit()
    {
        playerMelee.MeleeAttack();
    }
}
