using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSelectionMenu : MonoBehaviour
{
    public static UpgradeSelectionMenu instance;

    [SerializeField]
    GameObject upgradeSelectionMenu;
    [SerializeField]
    UpgradeUIElement upgradeUIElement;
    [SerializeField]
    Transform upgradeSpawnParent;

    List<UpgradeData> possibleUpgrades = new List<UpgradeData>();

    public List<UpgradeUIElement> generatedUpgrades = new List<UpgradeUIElement>();

    public static bool isUpgradeSelectionMenuOpen;

    public KeyCode OpenUpgradeSelectionMenuKey = KeyCode.T;

    public int numberOfUpgradeSelections, rerollCost;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(OpenUpgradeSelectionMenuKey))
        {
            OpenMenu();
        }
    }

    public void ToggleMenu()
    {
        if(isUpgradeSelectionMenuOpen)
        {
            CloseMenu();
        }
        else
            OpenMenu();
    }

    public void OpenMenu()
    {
        isUpgradeSelectionMenuOpen = true;
        upgradeSelectionMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        GenerateNewUpgrades();
    }

    public void CloseMenu()
    {
        isUpgradeSelectionMenuOpen = false;
        upgradeSelectionMenu.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        RemoveGeneratedUpgrades();
    }

    void GenerateNewUpgrades()
    {
        RefreshAvailableUpgrades();
        if (possibleUpgrades.Count < numberOfUpgradeSelections)
        {
            for (int i = 0; i < possibleUpgrades.Count; i++)
            {
                UpgradeUIElement clone = Instantiate(upgradeUIElement, upgradeSpawnParent);
                generatedUpgrades.Add(clone);
                clone.Init(GetUpgrade(), this);
            }
            possibleUpgrades.Clear();
            return;
        }


        for (int i = 0; i < numberOfUpgradeSelections; i++)
        {
            UpgradeUIElement clone = Instantiate(upgradeUIElement, upgradeSpawnParent);
            generatedUpgrades.Add(clone);
            clone.Init(GetUpgrade(), this);
        }
        possibleUpgrades.Clear();


    }

    void RemoveGeneratedUpgrades()
    {
        foreach (UpgradeUIElement upgrade in generatedUpgrades)
        {
            Destroy(upgrade.gameObject);
        }
        generatedUpgrades.Clear();
    }

    public void RerollUpgrades()
    {
        if(PointsManager.instance.currentPoints >= rerollCost)
        {
            PointsManager.instance.RemovePoints(rerollCost);
            RemoveGeneratedUpgrades();
            GenerateNewUpgrades();
        }
    }

    public void RefreshAvailableUpgrades()
    {
        possibleUpgrades.Clear();
        possibleUpgrades.AddRange(PlayerUpgrades.Instance.availableUpgrades);
    }

    UpgradeData GetUpgrade()
    {


        int rand = Random.Range(0, possibleUpgrades.Count);
        UpgradeData upgradeToReturn = possibleUpgrades[rand];

        possibleUpgrades.Remove(upgradeToReturn);

        return upgradeToReturn;
    }

    public void DisableButtonInteraction()
    {
        foreach(UpgradeUIElement upgrade in generatedUpgrades)
        {
            upgrade.gameObject.GetComponentInChildren<Button>().interactable = false;
        }
    }
}
