using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField] KeyCode meleeKey = KeyCode.V;


    [SerializeField] GameObject meleeWeaponHolder;
    [SerializeField] Transform meleePos;

    [SerializeField] float meleeCooldown, meleeWeaponReactivationTime;
    [SerializeField] int damage;
    bool canMelee;

    [Header("SFX")]
    [SerializeField] AudioSource meleeSFXSource;
    [SerializeField] AudioClip swapToMeleeSFX, hitEnemySFX;

    public static Action onMeleePerformed;
    // Start is called before the first frame update
    void Start()
    {
        canMelee = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.isPaused)
            CheckInputs();
    }

    void CheckInputs()
    {
        if (Input.GetKeyDown(meleeKey))
        {
            if(canMelee)
            {
                canMelee = false;
                onMeleePerformed?.Invoke();
                meleeWeaponHolder.SetActive(true);
                meleeSFXSource.PlayOneShot(swapToMeleeSFX);
                WeaponSwapping.instance.TemporarilyDeactivateWeapons(meleeWeaponHolder, meleeWeaponReactivationTime);
                //MeleeAttack();
                StartCoroutine(MeleeCooldown());
            }
        }
    }

    public void MeleeAttack()
    {
        Collider[] colliders;
        colliders = Physics.OverlapSphere(meleePos.position, 1);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("ZombieBody") || collider.CompareTag("ZombieHead"))
            {
                collider.GetComponentInParent<ZombieHealth>().TakeDamage(damage, false);
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(meleePos.position, 5);
    //}

    IEnumerator MeleeCooldown()
    {
        yield return new WaitForSeconds(meleeCooldown);
        canMelee = true;
        meleeWeaponHolder.SetActive(false);
    }
}
