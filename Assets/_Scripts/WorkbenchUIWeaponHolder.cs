using UnityEngine;
using UnityEngine.UI;

public class WorkbenchUIWeaponHolder : MonoBehaviour
{
    public Weapon weapon;
    [SerializeField] Image weaponImg;

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void InitialiseWeaponHolder(Weapon weaponToInitialise)
    {
        weapon = weaponToInitialise;
        weaponImg.sprite = weapon.UISprite;
        button.interactable = true;
    }

    public void SelectWeapon()
    {
        WeaponWorkbenchManager.instance.DisplayWeapon(weapon);
    }
}
