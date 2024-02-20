using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [SerializeField] List<GameObject> powerUpPrefabs = new List<GameObject>();

    List<PowerUpBase> activePowerUps = new List<PowerUpBase>();

    [SerializeField]
    GameObject powerUpUIElement;
    [SerializeField]
    TMP_Text powerUpNameText;
    Color powerUpNameTextStartingColour;

    [SerializeField]
    Transform activePowerUpContainer;

    activePowerUpUIElement activeInstantKillElement;

    Coroutine instaKillCoroutine;

    [SerializeField] int powerUpDuration;

    public static Action onInstantKillEnded;
    bool instantKillStatus;

    private void OnEnable()
    {
        ZombieHealth.dropPowerUp += SpawnPowerUp;
        onInstantKillEnded += InstantKillEnded;
    }

    void InstantKillEnded()
    {
        SetInstantKillStatus(false);
    }

    private void OnDisable()
    {
        ZombieHealth.dropPowerUp -= SpawnPowerUp;
        onInstantKillEnded -= InstantKillEnded;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        powerUpNameTextStartingColour = powerUpNameText.color;
    }

    public void SpawnPowerUp(Vector3 spawnLocation)
    {
        Instantiate(GetRandomPowerUp(), spawnLocation + new Vector3(0,.9f,0), new Quaternion(0, UnityEngine.Random.Range(0, 180), 0, 0));
    }

    GameObject GetRandomPowerUp()
    {
        return powerUpPrefabs[UnityEngine.Random.Range(0, powerUpPrefabs.Count)];
    }

    public void SetPowerUpActive(PowerUpBase powerUp)
    {
        switch (powerUp.powerUpType)
        {
            case PowerUpType.InstantKill:
                if (GetInstantKillStatus())
                {
                    activeInstantKillElement.RefreshDuration();
                    break;
                }
                SpawnPowerUpUIElement(powerUp);
                activePowerUps.Add(powerUp);
                SetInstantKillStatus(true);
                break;


        }
        SpawnPowerUpNameText(powerUp);
    }

    void SpawnPowerUpNameText(PowerUpBase activePowerUp)
    {
        powerUpNameText.text = activePowerUp.powerUpName; 
        powerUpNameText.transform.DOScale(1, .5f);
        powerUpNameText.DOColor(Color.clear, .3f).SetDelay(3).OnComplete(() => 
            { 
                powerUpNameText.transform.localScale = Vector3.zero;
                powerUpNameText.color = powerUpNameTextStartingColour;
            }
        );
    }

    void SpawnPowerUpUIElement(PowerUpBase _activePowerUp)
    {
        GameObject clone = Instantiate(powerUpUIElement, activePowerUpContainer);
        var activePowerUpUIElement = clone.GetComponent<activePowerUpUIElement>();
        activeInstantKillElement = activePowerUpUIElement;
        activePowerUpUIElement.Init(powerUpDuration, _activePowerUp.powerUpUIIcon, onInstantKillEnded);
    }

    void SetInstantKillStatus(bool status)
    {
        instantKillStatus = status;
    }

    public bool GetInstantKillStatus()
    {
        return instantKillStatus;
    }
}
