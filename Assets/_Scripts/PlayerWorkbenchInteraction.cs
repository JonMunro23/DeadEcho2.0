using UnityEngine;

public class PlayerWorkbenchInteraction : MonoBehaviour
{
    [Header("KeyBinds")]
    [SerializeField] KeyCode interactKey = KeyCode.E;

    [SerializeField] WeaponWorkbenchManager weaponWorkbenchManager;

    bool isWithinRange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isWithinRange)
        {
            if(Input.GetKeyDown(interactKey))
            {
                weaponWorkbenchManager.InitialiseWorkbenchUI();
                //Disable Movement
                //Disable Cam Movement
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("WeaponWorkbench"))
        {
            isWithinRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WeaponWorkbench"))
        {
            isWithinRange = false;
        }
    }
}
