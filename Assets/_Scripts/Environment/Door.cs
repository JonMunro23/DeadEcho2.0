using UnityEngine;

public class Door : MonoBehaviour
{
    public int doorCost;

    AudioSource doorOpeningSFXSource;
    Animator animator;
    [SerializeField]
    ZombieSpawnPoint[] spawnPointsToMakeAccessible;

    public bool isOpen;

    private void Awake()
    {
        doorOpeningSFXSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        tag = "Door";
    }

    public void OpenDoor()
    {
        isOpen = true;

        SetSpawnPointsAccessible();

        if(doorOpeningSFXSource != null)
            doorOpeningSFXSource.Play();

        animator.enabled = true;
    }

    void SetSpawnPointsAccessible()
    {
        foreach(ZombieSpawnPoint spawnPoint in spawnPointsToMakeAccessible)
        {
            spawnPoint.SetAccessible();
        }
    }
}
