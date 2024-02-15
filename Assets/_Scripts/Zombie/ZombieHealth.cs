using UnityEngine;
using System;

public class ZombieHealth : MonoBehaviour, IDamageable
{
    ZombieAI zombieAI;
    Animator animator;

    [SerializeField] int hitPoints, baseKillPoints, extraHeadshotPoints, startingHealth;
    [SerializeField] SkinnedMeshRenderer zombieHead;

    [SerializeField] GameObject zombieRagdoll;

    public int currentHealth;
    public bool isDead;

    public static Action onDeath;
    public static Action onHit;
    public static Action<Vector3> dropPowerUp;

    //int = damageTaken
    public static Action<int> onPointsGiven;
    [SerializeField] int powerUpDropChance;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        zombieAI = GetComponent<ZombieAI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int healthToRemove, bool hitHead = false)
    {
        if(!isDead)
        {
            currentHealth -= healthToRemove;
            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            onPointsGiven?.Invoke(hitPoints);
            onHit?.Invoke();
        }
        //else if(isDead)
        //{
        //    if (hitHead && zombieHead.enabled)
        //        ExplodeHead();
        //}
    }

    public void ExplodeHead()
    {
        zombieHead.enabled = false;
        if (!isDead)
        {
            onPointsGiven?.Invoke(extraHeadshotPoints);
            Die();
        }
        //play head explosing particle effect
        //play head explosing sfx
    }

    public void Die()
    {
        if(DoesSpawnPowerUp())
            dropPowerUp?.Invoke(transform.position);

        isDead = true;
        onDeath?.Invoke();
        onPointsGiven?.Invoke(baseKillPoints);
        zombieAI.isMoving = false;
        zombieAI.agent.velocity = Vector3.zero;
        zombieAI.agent.isStopped = true;
        animator.SetTrigger("Die");
        animator.SetBool("IsMoving", false);
        transform.SetParent(null);
        Destroy(zombieAI.agent);
        var ragdollClone = Instantiate(zombieRagdoll, transform.position, transform.rotation);
        //ragdollClone.GetComponent<Rigidbody>().AddForce(Vector3.back * 20 ,ForceMode.Impulse);
        Destroy(ragdollClone, 12);
        Destroy(gameObject);
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

    public void OnDamaged(int damageTaken, string hitBodyPart)
    {
        if (hitBodyPart == "ZombieHead")
            TakeDamage(damageTaken, true);
        else
            TakeDamage(damageTaken, false);
    }

    public void Kill()
    {
        Die();
    }
}
