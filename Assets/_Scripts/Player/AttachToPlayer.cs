using UnityEngine;

public class AttachToPlayer : MonoBehaviour
{
    [HideInInspector] public Transform cameraPosition;

    private void Awake()
    {
        cameraPosition = GameObject.FindGameObjectWithTag("PlayerCameraPos").transform;
    }

    private void Update()
    {
        transform.position = cameraPosition.position;
    }
}
