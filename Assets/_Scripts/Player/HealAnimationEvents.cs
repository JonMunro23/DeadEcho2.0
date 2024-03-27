using UnityEngine;

public class HealAnimationEvents : MonoBehaviour
{
    PlayerHealth playerHealth;
    AudioSource healSFXSource;
    [SerializeField] AudioClip healSFX;

    private void Awake()
    {
        healSFXSource = GetComponent<AudioSource>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    public void HealPlayer()
    {
        playerHealth.BeginHealthRegen();
        healSFXSource.PlayOneShot(healSFX);
    }
}
