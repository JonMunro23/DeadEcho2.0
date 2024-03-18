using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;
using System;

public class ZombieSpawnManager : MonoBehaviour
{
    [SerializeField]
    int remaningAmountToSpawn, totalAmountToSpawn, aliveZombies, zombiesKilledThisRound;
    bool canSpawnZombie, canSpawn;

    [SerializeField] List<ZombieSpawnPoint> activeSpawnPoints = new List<ZombieSpawnPoint>();
    [SerializeField] List<ZombieSpawnPoint> startingSpawnPoints = new List<ZombieSpawnPoint>();
    [SerializeField] GameObject zombieObj;
    [SerializeField] Transform zombieParent;
    [SerializeField] int maxNumberOfAliveZombies, startingAmountToSpawn;
    [SerializeField] float zombieSpawnCooldown;

    public bool canSpawnZombies = true;

    public static Action onAllZombiesKilled;

    private void OnEnable()
    {
        ZombieHealth.onDeath += KillZombie;
        ZombieSpawnPoint.onSpawnPointActivateStatusUpdated += UpdateSpawnPoint;
        RoundManager.onNewRoundStarted += StartSpawning;
    }
    private void OnDisable()
    {
        ZombieHealth.onDeath -= KillZombie;
        ZombieSpawnPoint.onSpawnPointActivateStatusUpdated -= UpdateSpawnPoint;
        RoundManager.onNewRoundStarted -= StartSpawning;
    }

    void Start()
    {
        totalAmountToSpawn = startingAmountToSpawn;
        remaningAmountToSpawn = totalAmountToSpawn;
        canSpawnZombie = true;
        //InitStartingSpawnPoints();

    }

    void Update()
    {
        if(canSpawn)
        {
            if(remaningAmountToSpawn == 0)
            {
                canSpawn = false;
            }

            if(aliveZombies < maxNumberOfAliveZombies)
            {
                TrySpawnZombie();
            }
        }
    }

    void StartSpawning(int currentRound)
    {
        if(canSpawnZombies)
        {
            totalAmountToSpawn = startingAmountToSpawn * currentRound;
            remaningAmountToSpawn = totalAmountToSpawn;
            zombiesKilledThisRound = 0;
            canSpawn = true;
        }
    }

    void TrySpawnZombie()
    {
        if(canSpawnZombie)
        {
            canSpawnZombie = false;
            remaningAmountToSpawn--;

            aliveZombies++;
            int rand = UnityEngine.Random.Range(0, activeSpawnPoints.Count);

            if (zombieParent)
                Instantiate(zombieObj, activeSpawnPoints[rand].transform.position, Quaternion.identity, zombieParent);

            StartCoroutine(SpawnCooldown());
        }
    }

    void InitStartingSpawnPoints()
    {
        foreach(ZombieSpawnPoint spawnPoint in startingSpawnPoints)
        {
            spawnPoint.SetIsActive(true);
        }
    }

    void UpdateSpawnPoint(ZombieSpawnPoint spawnPointToUpdate, bool _isSpawnPointActive)
    {
        //Debug.Log(_isSpawnPointActive);

        //if (spawnPointToUpdate == null)
        //    return;


        if(_isSpawnPointActive)
            activeSpawnPoints.Add(spawnPointToUpdate);
        else
            activeSpawnPoints.Remove(spawnPointToUpdate);
    }

    IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(zombieSpawnCooldown);
        canSpawnZombie = true;
    }

    void KillZombie(int playerIndex, bool wasHeadshot)
    {
        aliveZombies--;
        zombiesKilledThisRound++;
        if(zombiesKilledThisRound == totalAmountToSpawn)
        {
            onAllZombiesKilled?.Invoke();
        }
    }
}
