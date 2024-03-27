using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSelectionMenu : MonoBehaviour
{
    [SerializeField]
    GameObject upgradeSelectionMenu;
    [SerializeField]
    UpgradeUIElement upgradeUIElement;
    [SerializeField]
    Transform upgradeSpawnParent;

    public Upgrade[] possibleUpgrades;

    List<UpgradeUIElement> generatedUpgrades = new List<UpgradeUIElement>();

    public static bool isUpgradeSelectionMenuOpen;

    public KeyCode OpenUpgradeSelectionMenuKey = KeyCode.T;

    public int numberOfUpgradeSelections, rerollCost;

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
        for (int i = 0; i < numberOfUpgradeSelections; i++)
        {
            UpgradeUIElement clone = Instantiate(upgradeUIElement, upgradeSpawnParent);
            generatedUpgrades.Add(clone);
            clone.Init(GetUpgrade(), this);
        }
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

    Upgrade GetUpgrade()
    {
        int rand = Random.Range(0, possibleUpgrades.Length);
        return possibleUpgrades[rand];
    }

    public void DisableButtonInteraction()
    {
        foreach(UpgradeUIElement upgrade in generatedUpgrades)
        {
            upgrade.gameObject.GetComponentInChildren<Button>().interactable = false;
        }
    }
}
