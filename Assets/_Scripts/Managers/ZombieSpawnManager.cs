using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    int currentRound;

    private void OnEnable()
    {
        ZombieHealth.onDeath += KillZombie;
        ZombieSpawnPoint.onSpawnPointActivateStatusUpdated += UpdateSpawnPoint;
        RoundManager.onNewRoundStarted += StartSpawning;
        PlayerHealth.onDeath += StopSpawning;
    }
    private void OnDisable()
    {
        ZombieHealth.onDeath -= KillZombie;
        ZombieSpawnPoint.onSpawnPointActivateStatusUpdated -= UpdateSpawnPoint;
        RoundManager.onNewRoundStarted -= StartSpawning;
        PlayerHealth.onDeath -= StopSpawning;
    }

    void Start()
    {
        totalAmountToSpawn = startingAmountToSpawn;
        remaningAmountToSpawn = totalAmountToSpawn;
        canSpawnZombie = true;

    }

    void Update()
    {
        if(canSpawn)
        {
            if(remaningAmountToSpawn == 0)
            {
                StopSpawning();
            }

            if(aliveZombies < maxNumberOfAliveZombies)
            {
                TrySpawnZombie();
            }
        }
    }

    void StartSpawning(int _currentRound)
    {
        currentRound = _currentRound;

        if(canSpawnZombies)
        {
            totalAmountToSpawn = startingAmountToSpawn * _currentRound;
            remaningAmountToSpawn = totalAmountToSpawn;
            zombiesKilledThisRound = 0;
            canSpawn = true;
        }
    }

    void StopSpawning()
    {
        canSpawn = false;
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
            {
                if (rand < activeSpawnPoints.Count - 1)
                {
                    GameObject clone = Instantiate(zombieObj, activeSpawnPoints[rand].transform.position, Quaternion.identity, zombieParent);
                    if(clone.TryGetComponent<ZombieHealth>(out ZombieHealth zombieHealth))
                    {
                        zombieHealth.SetHealth(100 + (currentRound * 10));
                    }
                }
                else
                {
                    GameObject clone = Instantiate(zombieObj, activeSpawnPoints[0].transform.position, Quaternion.identity, zombieParent);
                    if (clone.TryGetComponent<ZombieHealth>(out ZombieHealth zombieHealth))
                    {
                        zombieHealth.SetHealth(100 + (currentRound * 10));
                    }
                }

            }

            StartCoroutine(SpawnCooldown());
        }
    }

    void UpdateSpawnPoint(ZombieSpawnPoint spawnPointToUpdate, bool _isSpawnPointActive)
    {
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
