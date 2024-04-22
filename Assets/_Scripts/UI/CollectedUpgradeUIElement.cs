using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectedUpgradeUIElement : MonoBehaviour
{
    [SerializeField]
    Image upgradeImage;
    [SerializeField]
    TMP_Text upgradeRankText;

    public void Init(Upgrade upgrade)
    {
        upgradeImage.sprite = upgrade.upgradeData.imageSprite;
        if(upgrade.currentUpgradeLevel == upgrade.upgradeData.maxUpgradeLevel)
            upgradeRankText.text = "Max";
        else
            upgradeRankText.text = upgrade.currentUpgradeLevel.ToString();
    }
}
