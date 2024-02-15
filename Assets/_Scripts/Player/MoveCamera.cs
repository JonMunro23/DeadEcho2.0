using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [HideInInspector] public Transform cameraPosition;

    private void Awake()
    {
        cameraPosition = GameObject.FindGameObjectWithTag("PlayerCameraPos").transform;
    }

    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
