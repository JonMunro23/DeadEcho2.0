using System;
using System.Collections;
using UnityEngine;

public class PowerUpBase : MonoBehaviour
{
    public PowerUpData powerUpData;
    public float timeVisible = 0.3f;
    public float timeInvisible = 0.3f;
    public float PickupRadius = .5f;
    
    public static Action<PowerUpBase> onPowerUpPickedUp;
    
    AudioSource audioSource;
    Coroutine despawnCoroutine;
    GameObject spawnedPrefab;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SpawnPrefab();
        SetPowerUpRadius();
        despawnCoroutine = StartCoroutine(Despawn());
    }

    void SpawnPrefab()
    {
        spawnedPrefab = Instantiate(powerUpData.prefab, transform.GetChild(0));
    }

    void SetPowerUpRadius()
    {
        GetComponent<SphereCollider>().radius = PickupRadius;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPowerUpPickedUp?.Invoke(this);
            GetComponent<Collider>().enabled = false;
            Destroy(spawnedPrefab);
            audioSource.PlayOneShot(powerUpData.pickupSFx);
            //play anim
            if (despawnCoroutine != null)
                StopCoroutine(despawnCoroutine);

            Destroy(gameObject, 2);
        }
    }


    //needs redone but is a step in the right direction
    IEnumerator Despawn()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        yield return new WaitForSeconds(powerUpData.litetime / 2);

        var whenAreWeDone = Time.time + powerUpData.litetime / 2;

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
