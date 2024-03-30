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

    UpgradeData upgrade;
    UpgradeSelectionMenu upgradeMenu;

    public void Init(UpgradeData _upgradeToInit, UpgradeSelectionMenu _upgradeMenu)
    {
        upgrade = _upgradeToInit;
        upgradeMenu = _upgradeMenu;

        if (_upgradeToInit.maxUpgradeLevel != 0)
            rank.text = "Rank : " + PlayerUpgrades.Instance.GetCurrentUpgradeRank(_upgradeToInit).ToString() + " / " + _upgradeToInit.maxUpgradeLevel;
        else
            rank.text = "Rank : " + PlayerUpgrades.Instance.GetCurrentUpgradeRank(_upgradeToInit).ToString();

        name.text = _upgradeToInit.name;
        description.text = _upgradeToInit.description;
        image.sprite = _upgradeToInit.imageSprite;
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
