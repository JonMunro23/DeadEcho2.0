using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDoorInteraction : MonoBehaviour
{
    [Header("KeyBinds")]
    [SerializeField] KeyCode interactKey = KeyCode.E;

    TMP_Text openDoorText;
    bool isWithinRange;
    Door doorToOpen;
    AudioSource purchaseSFXSource;
    private void Awake()
    {
        purchaseSFXSource = GameObject.FindGameObjectWithTag("PurchaseSFXSource").GetComponent<AudioSource>();
        openDoorText = GameObject.FindGameObjectWithTag("InteractText").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isWithinRange)
        {
            if(Input.GetKeyDown(interactKey))
            {
                if(PointsManager.instance.currentPoints >= doorToOpen.doorCost)
                {
                    PointsManager.instance.RemovePoints(doorToOpen.doorCost);
                    purchaseSFXSource.PlayOneShot(purchaseSFXSource.clip);
                    doorToOpen.OpenDoor();
                    doorToOpen = null;
                    isWithinRange = false;
                    openDoorText.text = "";
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Door"))
        {
            doorToOpen = other.GetComponentInParent<Door>();
            if(!doorToOpen.isOpen)
            {
                openDoorText.text = "Press " + interactKey.ToString() + " to open door for £" + doorToOpen.doorCost;
                isWithinRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            openDoorText.text = "";
            isWithinRange = false;
        }
    }
}
