using System;
using System.Collections;
using UnityEngine;

public class PlayerHealing : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField] KeyCode healKey = KeyCode.Q;

    PlayerHealth playerHealth;

    [SerializeField] GameObject syringeHolder;

    [SerializeField] float healCooldown, weaponReactivationTime;
    bool canHeal;

    [Header("SFX")]
    [SerializeField] AudioSource healSFXSource;
    [SerializeField] AudioClip swapToSyringeSFX;

    public float healAmount;

    public static Action onSyringeUsed;

    private void OnEnable()
    {
        PlayerHealth.onDeath += DisableHealing;
    }

    private void OnDisable()
    {
        PlayerHealth.onDeath -= DisableHealing;
    }

    private void Awake()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        canHeal = true;
    }

    void DisableHealing()
    {
        canHeal = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.isPaused)
            CheckInputs();
    }

    void CheckInputs()
    {
        if (Input.GetKeyDown(healKey))
        {
            if(canHeal && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                canHeal = false;
                onSyringeUsed?.Invoke();
                syringeHolder.SetActive(true);
                healSFXSource.PlayOneShot(swapToSyringeSFX);
                WeaponSwapping.instance.TemporarilyDeactivateWeapons(syringeHolder, weaponReactivationTime);
                StartCoroutine(HealCooldown());
            }
        }
    }

    IEnumerator HealCooldown()
    {
        yield return new WaitForSeconds(healCooldown);
        canHeal = true;
        syringeHolder.SetActive(false);
    }
}
