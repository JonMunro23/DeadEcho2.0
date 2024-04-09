using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth, currentMaxHealth, baseMaxHealth;
    [SerializeField] GameObject lowHealthOverlay;
    [SerializeField] Image healthbarImage;
    [SerializeField] float invincibilityLength;
    public float syringeHealAmount;

    bool canTakeDamage, isPlayingLowHealthSFX;
    Coroutine healthRegenCoRoutine;
    [SerializeField] AudioSource gettingHitSource, lowHealthSource;
    [SerializeField] AudioClip[] gettingHitSFX;

    public static Action onDeath, onDamageTaken;

    private void Awake()
    {
        healthbarImage = GameObject.FindGameObjectWithTag("HealthBarImage").GetComponent<Image>();
    }

    private void OnEnable()
    {
        PlayerUpgrades.onUpgradesRefreshed += UpdateMaxHealth;
    }

    private void OnDisable()
    {
        PlayerUpgrades.onUpgradesRefreshed -= UpdateMaxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        canTakeDamage = true;
        currentMaxHealth = baseMaxHealth;
        currentHealth = baseMaxHealth;
        isPlayingLowHealthSFX = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                TakeDamage(25);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage == true && currentHealth > 0)
        {
            canTakeDamage = false;
            currentHealth -= damage;
            onDamageTaken?.Invoke();
            UpdateHealthModifiers();
            gettingHitSource.PlayOneShot(GetRandomAudioClip());

            if (currentHealth <= 30)
            {
                if (!isPlayingLowHealthSFX)
                {
                    isPlayingLowHealthSFX = true;
                    lowHealthSource.Play();
                }
                lowHealthOverlay.SetActive(true);
            }

            if (currentHealth <= 0)
            {
                onDeath?.Invoke();

                if (healthRegenCoRoutine != null)
                    StopCoroutine(healthRegenCoRoutine);

                if (isPlayingLowHealthSFX)
                {
                    isPlayingLowHealthSFX = false;
                    lowHealthSource.Stop();
                }
                return;
            }

            StartCoroutine(Invincibility());
           
        }

    }

    void UpdateHealthModifiers()
    {
        healthbarImage.fillAmount = currentHealth / currentMaxHealth;

    }

    public void UpdateMaxHealth()
    {
        currentMaxHealth = baseMaxHealth;
        currentMaxHealth += baseMaxHealth * PlayerUpgrades.maxHealthModifier;
        UpdateHealthModifiers();
    }

    AudioClip GetRandomAudioClip()
    {
        int rand = UnityEngine.Random.Range(0, gettingHitSFX.Length);
        return gettingHitSFX[rand];
    }

    IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(invincibilityLength);
        canTakeDamage = true;
    }

    public void BeginHealthRegen()
    {
        healthRegenCoRoutine = StartCoroutine(HealthRegen(syringeHealAmount + syringeHealAmount * PlayerUpgrades.healthRecoverModifier));
    }

    IEnumerator HealthRegen(float regenAmount)
    {
        float healedAmount = 0;
        while (healedAmount < regenAmount && currentHealth < currentMaxHealth)
        {
            healedAmount += currentMaxHealth / currentMaxHealth;
            //Debug.Log(healedAmount);
            currentHealth += currentMaxHealth / currentMaxHealth;
            UpdateHealthModifiers();
            if (currentHealth >= 30)
            {
                if (isPlayingLowHealthSFX)
                {
                    StartCoroutine(_Helpers.FadeOutAudio(lowHealthSource, 2));
                    isPlayingLowHealthSFX = false;
                }
                lowHealthOverlay.SetActive(false);
            }
            yield return new WaitForSeconds(.04f);
        }
        healthRegenCoRoutine = null;
    }
}
