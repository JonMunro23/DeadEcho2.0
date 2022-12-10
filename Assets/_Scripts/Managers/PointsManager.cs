using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager instance;

    [Header("Keybinds")]
    [SerializeField] KeyCode addMoreCash = KeyCode.Alpha3;

    [SerializeField] TMP_Text currentPointsText;

    public int currentPoints;
    int totalPointsEarned;

    [SerializeField] int startingPointsValue;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPoints = startingPointsValue;
        UpdatePointsText();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(Input.GetKeyDown(addMoreCash))
            {
                AddPoints(1000);
            }
        }
    }

    public void AddPoints(int pointsToAdd)
    {
        currentPoints += pointsToAdd;
        totalPointsEarned += pointsToAdd;
        UpdatePointsText();
    }

    public void RemovePoints(int pointsToRemove)
    {
        currentPoints -= pointsToRemove;
        UpdatePointsText();
    }

    void UpdatePointsText()
    {
        currentPointsText.text = "£" + currentPoints.ToString();
    }
}
