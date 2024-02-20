using System.Collections;
using System.Collections.Generic;
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
        
    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage == true)
        {
            canTakeDamage = false;
            currentHealth -= damage;
            healthbarImage.fillAmount = currentHealth / 100;
            gettingHitSource.PlayOneShot(GetRandomAudioClip());

            //Display hit ui

            if (currentHealth <= 0)
            {
                //Time.timeScale = 0;
                //PauseMenu.isPaused = true;
                //Cursor.lockState = CursorLockMode.None;
                //_gameOverOverlay.SetActive(true);
                //_gameOverOverlay.transform.parent.GetChild(1).GetChild(0).transform.gameObject.SetActive(true);
                //scoreBoard.SetActive(true);
                Debug.Log("Player is Dead");
            }

            StartCoroutine(Invincibility());
            
            if (healthRegenCoRoutine != null)
                StopCoroutine(healthRegenCoRoutine);

            healthRegenCoRoutine = StartCoroutine(HealthRegen());
        }

        if (currentHealth <= 30)
        {
            if (!isPlayingLowHealthSFX)
            {
                isPlayingLowHealthSFX = true;
                lowHealthSource.Play();
            }
            lowHealthOverlay.SetActive(true);
        }
    }

    AudioClip GetRandomAudioClip()
    {
        int rand = Random.Range(0, gettingHitSFX.Length);
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
