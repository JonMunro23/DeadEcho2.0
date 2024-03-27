using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PowerUpType
{
    MaxAmmo,
    InstantKill,
    DoublePoints,
    BottomlessClip
}

public class PowerUpBase : MonoBehaviour
{
    [HideInInspector]
    public string powerUpName;

    public Sprite powerUpUIIcon;

    [SerializeField]
    AudioClip pickupSfx;
    AudioSource audioSource;

    [SerializeField]
    int totalLifetime = 30;

    public PowerUpType powerUpType;

    public float timeVisible = 0.3f;
    public float timeInvisible = 0.3f;

    public Action endAction;

    Coroutine blinkCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetPowerUpDisplayName();
        //StartCoroutine(StartDespawnTimer());
        blinkCoroutine = StartCoroutine(blink());
    }

    void SetPowerUpDisplayName()
    {
        switch (powerUpType)
        {
            case PowerUpType.MaxAmmo:
                powerUpName = "Max Ammo";
                break;
            case PowerUpType.InstantKill:
                powerUpName = "Insta-Kill";
                break;
            case PowerUpType.DoublePoints:
                powerUpName = "Double Points";
                break;
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            Destroy(transform.GetChild(0).gameObject);
            audioSource.PlayOneShot(pickupSfx);
            //play anim
            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);
            Destroy(gameObject, 2);
        }
    }

    public IEnumerator StartDespawnTimer()
    {
        int counter = totalLifetime;
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        while (counter > -1)
        {
            if (counter <= totalLifetime / 2)
                foreach (MeshRenderer renderer in meshRenderers)
                { 
                    renderer.enabled = !renderer.enabled;
                }
            yield return new WaitForSeconds(1);
            counter--;
        }
        Destroy(gameObject);
    }


    //needs redone but is a step in the right direction
    IEnumerator blink()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        yield return new WaitForSeconds(totalLifetime / 2);

        var whenAreWeDone = Time.time + totalLifetime / 2;

        while (Time.time < whenAreWeDone)
        {
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.enabled = false;
            }
            yield return new WaitForSeconds(timeInvisible);
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.enabled = true;
            }
            yield return new WaitForSeconds(timeVisible);
            
        }

        Destroy(gameObject);


    }
}
