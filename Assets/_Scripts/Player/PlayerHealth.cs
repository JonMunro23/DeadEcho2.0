using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float startingHealth;
    public float currentHealth, maxHealth;
    [SerializeField] GameObject lowHealthOverlay;
    [SerializeField] Image healthbarImage;
    [SerializeField] float invincibilityLength;

    bool canTakeDamage, isPlayingLowHealthSFX;
    Coroutine healthRegenCoRoutine;
    [SerializeField] AudioSource gettingHitSource, lowHealthSource;
    [SerializeField] AudioClip[] gettingHitSFX;

    public static Action onDeath;

    private void Awake()
    {
        healthbarImage = GameObject.FindGameObjectWithTag("HealthBarImage").GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        canTakeDamage = true;
        maxHealth = startingHealth;
        currentHealth = maxHealth;
        isPlayingLowHealthSFX = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                TakeDamage(maxHealth / 4);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage == true && currentHealth > 0)
        {
            canTakeDamage = false;
            currentHealth -= damage;
            healthbarImage.fillAmount = currentHealth / 100;
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

                if(healthRegenCoRoutine != null)
                    StopCoroutine(healthRegenCoRoutine);

                if(isPlayingLowHealthSFX)
                {
                    isPlayingLowHealthSFX = false;
                    lowHealthSource.Stop();
                }
                return;
            }

            StartCoroutine(Invincibility());
            
            if (healthRegenCoRoutine != null)
                StopCoroutine(healthRegenCoRoutine);

            healthRegenCoRoutine = StartCoroutine(HealthRegen());

        }

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



    IEnumerator HealthRegen()
    {
        yield return new WaitForSeconds(5);
        while (currentHealth < maxHealth)
        {
            currentHealth += maxHealth / 100;
            healthbarImage.fillAmount = currentHealth / 100;
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
