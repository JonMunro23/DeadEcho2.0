using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenade : MonoBehaviour
{
    public GameObject grenade;
    [SerializeField] Transform grenadeSpawnLocation;

    [SerializeField] float throwForce;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPullPinSFX()
    {
        audioSource.Play();
    }

    public void SpawnGrenade()
    {
        float randX = Random.Range(-360, 360);
        float randY = Random.Range(-360, 360);
        float randZ = Random.Range(-360, 360);

        float randTorque = Random.Range(-5f, 5f);

        GameObject clone = Instantiate(grenade, grenadeSpawnLocation.position, Quaternion.identity);
        clone.GetComponent<Rigidbody>().AddForce(throwForce * grenadeSpawnLocation.forward, ForceMode.Impulse);
        clone.GetComponent<Rigidbody>().AddTorque(randTorque * new Vector3(randX, randY, randZ), ForceMode.Impulse);
    }
}
