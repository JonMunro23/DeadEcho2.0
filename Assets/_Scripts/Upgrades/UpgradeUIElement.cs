using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIElement : MonoBehaviour
{
    [SerializeField]
    new TMP_Text name;
    [SerializeField]
    TMP_Text description, rank;
    [SerializeField]
    Image image;

    Upgrade upgrade;
    UpgradeSelectionMenu upgradeMenu;

    public void Init(Upgrade _upgradeToInit, UpgradeSelectionMenu _upgradeMenu)
    {
        upgrade = _upgradeToInit;
        upgradeMenu = _upgradeMenu;

        if (_upgradeToInit.upgradeData.maxUpgradeLevel != 0)
            rank.text = "Rank : " + _upgradeToInit.currentUpgradeLevel + " / " + _upgradeToInit.upgradeData.maxUpgradeLevel;
        else
            rank.text = "Rank : " + _upgradeToInit.currentUpgradeLevel;

        name.text = _upgradeToInit.upgradeData.name;
        description.text = _upgradeToInit.upgradeData.description;
        image.sprite = _upgradeToInit.upgradeData.imageSprite;
    }

    public void SelectUpgrade()
    {
        upgradeMenu.DisableButtonInteraction();
        PlayerUpgrades.Instance.AddUpgradeToCollection(upgrade);
        transform.DOScale(1.2f, .6f).SetUpdate(true).OnComplete(() =>
        {
            upgradeMenu.CloseMenu();
        });
    }
}
