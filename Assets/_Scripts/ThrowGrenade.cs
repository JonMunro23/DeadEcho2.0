using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenade : MonoBehaviour
{
    public GameObject grenade;
    [SerializeField] Transform grenadeSpawnLocation;

    [SerializeField] float throwForce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
