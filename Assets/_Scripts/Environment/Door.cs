using UnityEngine;

public class Door : MonoBehaviour
{
    public int doorCost;
    [SerializeField] GameObject doorObject;

    AudioSource doorOpeningSFXSource;
    Animation animationComponent;
    [SerializeField]
    ZombieSpawnPoint[] spawnPointsToMakeAccessible;



    public bool isOpen;

    [Header("Animation")]
    [Tooltip("if false will destory doorObject and its children")]
    [SerializeField] bool playsAnimation;

    private void Awake()
    {
        doorOpeningSFXSource = GetComponent<AudioSource>();
        animationComponent = GetComponent<Animation>();
        doorObject = gameObject;
    }

    public void OpenDoor()
    {
        isOpen = true;

        SetSpawnPointsAccessible();

        if(doorOpeningSFXSource != null)
            doorOpeningSFXSource.Play();

        if(playsAnimation)
        {
            animationComponent.Play();
        }
        else
        {
            Destroy(doorObject);
            Destroy(this.gameObject, 5);
        }

    }

    void SetSpawnPointsAccessible()
    {
        foreach(ZombieSpawnPoint spawnPoint in spawnPointsToMakeAccessible)
        {
            spawnPoint.SetAccessible();
        }
    }
}
