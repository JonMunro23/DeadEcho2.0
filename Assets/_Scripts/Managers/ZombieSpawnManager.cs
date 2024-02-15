using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ZombieSpawnManager : MonoBehaviour
{
    int amountToSpawn, aliveZombies;
    bool canSpawnZombie, canStartNextWave;

    [SerializeField] List<GameObject> spawnPoints = new List<GameObject>();
    [SerializeField] GameObject zombieObj, playerCrosshair;
    [SerializeField] Transform zombieParent;
    [SerializeField] int maxNumberOfAliveZombies, startingAmountToSpawn;
    [SerializeField] float zombieSpawnCooldown, gracePeriodLength;
    TMP_Text currentWaveText, waveText;

    public int currentRound;

    public bool canSpawnZombies = true;

    private void Awake()
    {
        currentWaveText = GameObject.FindGameObjectWithTag("CurrentRoundText").GetComponent<TMP_Text>();
        waveText = currentWaveText.transform.parent.GetChild(1).GetComponent<TMP_Text>();
    }
    private void OnEnable()
    {
        ZombieHealth.onDeath += KillZombie;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(canSpawnZombies)
        {
            amountToSpawn = startingAmountToSpawn;
            canSpawnZombie = true;
            spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("ZombieSpawnPoint"));
            currentRound = 1;
            UpdateCurrentWaveText();
            PlayWaveTextIntroAnimation();
        }
    }

    private void OnDisable()
    {
        ZombieHealth.onDeath -= KillZombie;
    }
    // Update is called once per frame
    void Update()
    {
        if(spawnPoints.Count > 0)
        {
            while (amountToSpawn > 0 && canSpawnZombie && aliveZombies < maxNumberOfAliveZombies)
            {
                canSpawnZombie = false;
                amountToSpawn--;
                aliveZombies++;
                int rand = Random.Range(0, spawnPoints.Count);

                if(zombieParent)
                    Instantiate(zombieObj, spawnPoints[rand].transform.position, Quaternion.identity, zombieParent);
                else
                    Instantiate(zombieObj, spawnPoints[rand].transform.position, Quaternion.identity);

                StartCoroutine(SpawnCooldown());
            }
        }

        if (amountToSpawn == 0 && aliveZombies == 0 && canStartNextWave)
        {
            NextRound();
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                NextRound();
            }
        }
    }

    void NextRound()
    {
        canStartNextWave = false;
        PlayWaveTextNextWaveAnimation();
        //StartCoroutine(NewRoundGracePeriod());
        //play new round UI anims
        //play new round jingle
    }

    //IEnumerator NewRoundGracePeriod()
    //{
    //    yield return new WaitForSeconds(gracePeriodLength);
    //    amountToSpawn = startingAmountToSpawn * currentRound;
    //    canStartNextWave = true;
    //}

    IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(zombieSpawnCooldown);
        canSpawnZombie = true;
    }

    void KillZombie()
    {
        aliveZombies--;
    }

    void UpdateCurrentWaveText()
    {
        currentWaveText.text = currentRound.ToString();
    }

    void PlayWaveTextIntroAnimation()
    {
        currentWaveText.DOColor(Color.red, 2).OnComplete(() => 
        {
            currentWaveText.transform.DOScale(.75f, 1).SetEase(Ease.OutCirc);
            waveText.DOColor(Color.clear, .3f).OnComplete(() =>
                {
                    Destroy(waveText);
                }
            );

            currentWaveText.GetComponent<RectTransform>().DOLocalMove(new Vector3(50, -50, 0), 1).SetDelay(.35f).SetEase(Ease.OutCirc).OnComplete(() =>
            {
                canStartNextWave = true;
            });
        });
        
    }

    void PlayWaveTextNextWaveAnimation()
    {
        currentWaveText.DOColor(Color.clear, 2.5f).OnComplete(() => 
        { 
            canStartNextWave = true;
            currentRound++;
            amountToSpawn = startingAmountToSpawn * currentRound;
            UpdateCurrentWaveText();
            currentWaveText.DOColor(Color.red, 2.5f).OnComplete(() =>
            {
                currentWaveText.DOColor(Color.white, 1f).SetLoops(4, LoopType.Yoyo);
                canStartNextWave = true;
            });
        }
        );
    }
}
