using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerThrowables : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField] KeyCode throwEquipmentKey = KeyCode.G;
    
    [SerializeField] GameObject grenadeArmsHolder, grenadeObj;
    [SerializeField] Transform grenadeSpawnLocation;

    public bool canThrowGrenade;

    [SerializeField] int startingGrenadeCount;
    [SerializeField] float grenadeReactivationTime;
    [HideInInspector] public int currentGrenadeCount;

   TMP_Text currentGrenadeCountText;

    public static Action onEquipmentUsed;

    private void Awake()
    {
        currentGrenadeCountText = GameObject.FindGameObjectWithTag("EquipmentOwnedText").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        currentGrenadeCount = startingGrenadeCount;
        canThrowGrenade = true;
        currentGrenadeCountText.text = currentGrenadeCount.ToString();
    }


    // Update is called once per frame
    void Update()
    {
        if(canThrowGrenade && currentGrenadeCount > 0)
        {
            if(Input.GetKeyDown(throwEquipmentKey))
            {
                canThrowGrenade = false;
                onEquipmentUsed?.Invoke();
                currentGrenadeCount--;
                currentGrenadeCountText.text = currentGrenadeCount.ToString();
                grenadeArmsHolder.SetActive(true);
                WeaponSwapping.instance.TemporarilyDeactivateWeapons(grenadeArmsHolder, grenadeReactivationTime);
                StartCoroutine(ThrowGrenadeCooldown());
            }
        }
    }

    public void AddGrenades(int amountToAdd)
    {
        currentGrenadeCount += amountToAdd;
        currentGrenadeCountText.text = currentGrenadeCount.ToString();
    }

    IEnumerator ThrowGrenadeCooldown()
    {
        yield return new WaitForSeconds(2);
        canThrowGrenade = true;
    }

}
