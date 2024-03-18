using UnityEngine;
using System;

public class ZombieHealth : MonoBehaviour, IDamageable
{
    ZombieAI zombieAI;
    Animator animator;

    [SerializeField] int hitPoints, baseKillPoints, extraHeadshotPoints;
    [SerializeField] SkinnedMeshRenderer zombieHead;

    [SerializeField] GameObject zombieRagdoll;

    public int currentHealth;
    public bool isDead;

    [Header("Sound")]
    public AudioClip[] hitSFx;
    AudioSource gettngHitAudioSource;

    //int = index of player that killed, bool = wasHeadshot
    public static Action<int, bool> onDeath;
    public static Action onHit;
    //vector3 = drop location
    public static Action<Vector3> dropPowerUp;

    //int = damageTaken
    public static Action<int> onPointsGiven;
    [SerializeField] int powerUpDropChance;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        zombieAI = GetComponent<ZombieAI>();
        gettngHitAudioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //SetHealth(startingHealth);
    }

    public void TakeDamage(int healthToRemove, bool hitHead)
    {
        if(!isDead)
        {
            currentHealth -= healthToRemove;
            if (currentHealth <= 0)
            {
                if(hitHead)
                {
                    ExplodeHead();
                    return;
                }

                Die(hitHead);
                return;
            }

            onPointsGiven?.Invoke(hitPoints);
            onHit?.Invoke();
        }
    }

    public void ExplodeHead()
    {
        zombieHead.enabled = false;
        if (!isDead)
        {
            onPointsGiven?.Invoke(extraHeadshotPoints);
            Die(true);
        }
        //play head explosing particle effect
        //play head explosing sfx
    }

    public void Die(bool wasHeadshot)
    {
        if(DoesSpawnPowerUp())
            dropPowerUp?.Invoke(transform.position);

        isDead = true;
        onDeath?.Invoke(0, wasHeadshot);
        onPointsGiven?.Invoke(baseKillPoints);
        zombieAI.isMoving = false;
        zombieAI.agent.velocity = Vector3.zero;
        zombieAI.agent.isStopped = true;
        animator.SetTrigger("Die");
        animator.SetBool("IsMoving", false);
        transform.SetParent(null);
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }    

        Destroy(zombieAI.agent);
        //var ragdollClone = Instantiate(zombieRagdoll, transform.position, transform.rotation);
        //Destroy(ragdollClone, 12);
        Destroy(gameObject, 12);
        //add chance to gib depending on weapon?
    }

    bool DoesSpawnPowerUp()
    {
        int randInt = UnityEngine.Random.Range(0, 100);
        if (randInt <= powerUpDropChance)
            return true;
        else
            return false;

    }

    public void SetHealth(int health)
    {
        currentHealth = health;
    }

    public void OnDamaged(int damageTaken, bool wasHeadshot)
    {
        TakeDamage(damageTaken, wasHeadshot);
    }

    public void InstantlyKill()
    {
        Die(false);
    }
}
