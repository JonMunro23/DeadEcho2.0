using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] float smooth, swayMultiplier;

    public static bool isWeaponSwayActive;

    Quaternion baseRotation;
    private void Start()
    {
        isWeaponSwayActive = true;
        baseRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(isWeaponSwayActive)
        {
            float MouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
            float MouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

            Quaternion rotationX = Quaternion.AngleAxis(-MouseY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(MouseX, Vector3.up);

            Quaternion targetRotation = rotationX * rotationY;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
        else
        {
            transform.localRotation = baseRotation;
        }
    }
}
