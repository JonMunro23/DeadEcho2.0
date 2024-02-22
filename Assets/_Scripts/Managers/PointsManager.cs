using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager instance;

    [Header("Keybinds")]
    [SerializeField] KeyCode addMoreCash = KeyCode.Alpha3;

    TMP_Text currentPointsText;

    public int currentPoints;
    int totalPointsEarned;

    [SerializeField] int startingPointsValue;

    [SerializeField] Transform gainedPointsTextSpawnLocation;
    public GameObject gainedPointsText;
    //int = player points to update, int = updatatedPoints
    public static Action<int, int> pointsUpdated;

    private void Awake()
    {
        instance = this;
        currentPointsText = GameObject.FindGameObjectWithTag("CurrentPointsText").GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        ZombieHealth.onPointsGiven += AddPoints;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentPoints = startingPointsValue;
        UpdatePointsText();
    }
    private void OnDisable()
    {
        ZombieHealth.onPointsGiven -= AddPoints;
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKey(addMoreCash))
            {
                AddPoints(10);
            }
        }
    }

    public void AddPoints(int pointsToAdd)
    {
        currentPoints += pointsToAdd;
        totalPointsEarned += pointsToAdd;
        SpawnGainedPointsText(pointsToAdd);
        UpdatePointsText();
        pointsUpdated?.Invoke(0, pointsToAdd);
    }

    public void RemovePoints(int pointsToRemove)
    {
        currentPoints -= pointsToRemove;
        UpdatePointsText();
        //pointsUpdated?.Invoke(0, -pointsToRemove);
    }

    void UpdatePointsText()
    {
        currentPointsText.text = "£" + currentPoints.ToString();
        
    }

    void SpawnGainedPointsText(int gainedPoints)
    {
        float randXOffset = UnityEngine.Random.Range(-5f, 5f);
        float randYOffset = UnityEngine.Random.Range(-5f, 5f);
        Vector2 spawnLocation = new Vector2(gainedPointsTextSpawnLocation.position.x + randXOffset, gainedPointsTextSpawnLocation.position.y + randYOffset);
        GameObject clone = Instantiate(gainedPointsText, spawnLocation, Quaternion.identity, gainedPointsTextSpawnLocation);
        TMP_Text text = clone.GetComponent<TMP_Text>();
        text.text = "+" + gainedPoints.ToString();
        clone.GetComponent<Rigidbody2D>().AddForce(Vector2.right * UnityEngine.Random.Range(50, 80 + 1), ForceMode2D.Impulse);
        clone.GetComponent<Rigidbody2D>().AddForce(Vector2.up * UnityEngine.Random.Range(-15, 15 + 1), ForceMode2D.Impulse);
        text.DOColor(Color.clear, .2f).SetDelay(1).OnComplete(() =>
            {
                Destroy(clone);
            }
        );
    }
}
