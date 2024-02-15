using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HitMarkerManager : MonoBehaviour
{
    public static HitMarkerManager instance;

    GameObject hitMarker;

    [SerializeField] AudioSource hitmarkerSFXSource;
    [SerializeField] AudioClip hitMarkerSfx;
    Image[] hitmarkerArms;

    Coroutine deactivateHitMarker;

    private void Awake()
    {
        instance = this;
        hitMarker = GameObject.FindGameObjectWithTag("HitMarker");
    }
    private void OnEnable()
    {
        ZombieHealth.onDeath += ShowKillHitmarker;
        ZombieHealth.onHit += ShowNormalHitmarker;
    }

    private void OnDisable()
    {
        ZombieHealth.onDeath -= ShowKillHitmarker;
        ZombieHealth.onHit -= ShowNormalHitmarker;
    }

    void ShowKillHitmarker()
    {
        ActivateHitMarker(Color.red);
    }

    void ShowNormalHitmarker()
    {
        ActivateHitMarker(Color.white);
    }

    public void ActivateHitMarker(Color hitmarkerColour)
    {
        hitMarker.transform.GetChild(0).gameObject.SetActive(true);
        SetHitMarkerArmColour(hitmarkerColour);
        if(hitmarkerSFXSource.isPlaying) hitmarkerSFXSource.Stop();
        hitmarkerSFXSource.PlayOneShot(hitMarkerSfx);
        if (deactivateHitMarker != null)
            StopCoroutine(deactivateHitMarker);

        deactivateHitMarker = StartCoroutine(DeactivateHitMarker());
    }

    void SetHitMarkerArmColour(Color color)
    {
        hitmarkerArms = hitMarker.transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < hitmarkerArms.Length; i++)
        {
            hitmarkerArms[i].color = color;
        }
    }

    IEnumerator DeactivateHitMarker()
    {
        yield return new WaitForSeconds(.1f);
        hitMarker.transform.GetChild(0).gameObject.SetActive(false);
    }
}
