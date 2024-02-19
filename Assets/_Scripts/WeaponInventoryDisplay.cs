using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventoryDisplay : MonoBehaviour
{
    [SerializeField]
    Image primary1Slot, primary2Slot, secondarySlot;
    [SerializeField]
    Image primary1SlotContainer, primary2SlotContainer, secondarySlotContainer;
    [SerializeField]
    Color selectedSlotColour, unselectedSlotColour, noAmmoColour;

    public void Init(int currentlyEquippedWeaponSlot, Weapon secondary, Weapon primary1 = null, Weapon primary2 = null)
    {
        if (primary1 != null)
        {
            primary1Slot.color = Color.white;
            primary1Slot.sprite = primary1.UISprite;
        }
        else
            primary1Slot.color = Color.clear;

        if (primary2 != null)
        {
            primary2Slot.color = Color.white;
            primary2Slot.sprite = primary2.UISprite;
        }
        else
            primary2Slot.color = Color.clear;

        if (secondary != null)
        {
            secondarySlot.color = Color.white;
            secondarySlot.sprite = secondary.UISprite;
        }else
            secondarySlot.color = Color.clear;

        UpdateSelectedSlot(currentlyEquippedWeaponSlot);
    }

    public void UpdateSelectedSlot(int selectedSlot)
    {
        switch (selectedSlot)
        {
            case 1:
                primary1SlotContainer.color = selectedSlotColour;
                primary2SlotContainer.color = unselectedSlotColour;
                secondarySlotContainer.color = unselectedSlotColour;
                break;
            case 2:
                primary2SlotContainer.color = selectedSlotColour;
                primary1SlotContainer.color = unselectedSlotColour;
                secondarySlotContainer.color = unselectedSlotColour;
                break;
            case 3:
                secondarySlotContainer.color = selectedSlotColour;
                primary1SlotContainer.color = unselectedSlotColour;
                primary2SlotContainer.color = unselectedSlotColour;
                break;
        }
    }

    public void UpdateSlotAmmo(int slotToUpdate, int newCurrentAmmo, int newReserveAmmo)
    {

    }
}
