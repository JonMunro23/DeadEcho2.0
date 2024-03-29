using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIElement : MonoBehaviour
{
    [SerializeField]
    new TMP_Text name;
    [SerializeField]
    TMP_Text description;
    [SerializeField]
    Image image;

    Upgrade upgrade;
    UpgradeSelectionMenu upgradeMenu;

    public void Init(Upgrade _upgradeToInit, UpgradeSelectionMenu _upgradeMenu)
    {
        upgrade = _upgradeToInit;
        upgradeMenu = _upgradeMenu;

        name.text = _upgradeToInit.name;
        description.text = _upgradeToInit.description;
        image.sprite = _upgradeToInit.imageSprite;
    }

    public void SelectUpgrade()
    {
        upgradeMenu.DisableButtonInteraction();
        upgradeMenu.AddUpgradeToCollection(upgrade);
        transform.DOScale(1.2f, .6f).SetUpdate(true).OnComplete(() =>
        {
            upgradeMenu.CloseMenu();
        });
    }
}
