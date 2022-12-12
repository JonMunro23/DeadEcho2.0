using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    ZombieMovement zombieMovement;
    [SerializeField] int startingHealth;
    public int currentHealth;

    [SerializeField] int hitPoints, baseKillPoints, extraHeadshotPoints;

    [SerializeField] SkinnedMeshRenderer zombieHead;

    Animator animator;

    public bool isDead;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        zombieMovement = GetComponent<ZombieMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveHealth(int healthToRemove, bool hitHead)
    {
        if(!isDead)
        {
            PointsManager.instance.AddPoints(hitPoints);
            currentHealth -= healthToRemove;
            if(currentHealth <= 0)
            {
                if (hitHead)
                    ExplodeHead();
                else
                    Die();
            }
        }
        else if(isDead)
        {
            if (hitHead && zombieHead.enabled)
                ExplodeHead();
        }
    }

    public void ExplodeHead()
    {
        zombieHead.enabled = false;
        if (!isDead)
        {
            PointsManager.instance.AddPoints(extraHeadshotPoints);
            Die();
        }
        //play head explosing particle effect
        //play head explosing sfx
    }

    public void Die()
    {
        PointsManager.instance.AddPoints(baseKillPoints);
        isDead = true;
        zombieMovement.isMoving = false;
        zombieMovement.agent.velocity = Vector3.zero;
        zombieMovement.agent.isStopped = true;
        animator.SetTrigger("Die");
        animator.SetBool("IsMoving", false);
        //Destroy(zombieMovement.agent);
        Destroy(gameObject, 60);
        //add chance to gib depending on weapon?
    }
}
