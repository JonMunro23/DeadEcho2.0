using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class ZombieSpawnManager : MonoBehaviour
{
    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();

    [SerializeField] GameObject zombieObj;
    [SerializeField] Transform zombieParent;

    [SerializeField] int maxNumberOfZombies, amountToSpawn;
    [SerializeField] float zombieSpawnCooldown;
    bool canSpawnZombie;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        canSpawnZombie = true;
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("ZombieSpawnPoint"));
    }

    // Update is called once per frame
    void Update()
    {
        while (amountToSpawn > 0 && canSpawnZombie)
        {
            canSpawnZombie = false;
            amountToSpawn--;
            int rand = Random.Range(0, spawnPoints.Count);
            Instantiate(zombieObj, spawnPoints[rand].transform.position, Quaternion.identity, zombieParent);
            StartCoroutine(SpawnCooldown());
        }
    }

    IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(zombieSpawnCooldown);
        canSpawnZombie = true;
    }
}
