using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieMovement : MonoBehaviour
{
    ZombieHealth zombieHealth;
    public NavMeshAgent agent;
    Animator animator;
    [SerializeField]
    Transform meleePos, playerPosition;
    public float speed, meleeCooldown, meleePerformTime, distanceToPerformMeleeAttack;
    public int damage;
    public bool isDead, isMoving, isAttacking, canPerformMeleeAttack = true;

    private void Awake()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        zombieHealth = GetComponent<ZombieHealth>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        agent.speed = speed;
        agent.height = 1.82f;
        agent.baseOffset = -.065f;
        SetTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (!zombieHealth.isDead)
        {
            if (!isAttacking)
                SetTarget();

            if (isMoving)
            {
                animator.SetBool("IsMoving", isMoving);
            }
            if (Vector3.Distance(transform.position, agent.destination) <= distanceToPerformMeleeAttack && canPerformMeleeAttack == true)
            {
                //MeleeAttack();
                //Debug.Log("Performed Melee Attack");
            }
        }
    }

    private void SetTarget()
    {
        agent.SetDestination(playerPosition.position);
        agent.isStopped = false;
        isMoving = true;
        animator.SetBool("IsMoving", isMoving);
    }

    void MeleeAttack()
    {
        isAttacking = true;
        canPerformMeleeAttack = false;
        agent.isStopped = true;
        isMoving = false;
        animator.SetBool("IsMoving", isMoving);
        Collider[] colliders;
        animator.SetTrigger("MeleeAttack");
        StartCoroutine(MeleeAttackPerformTime());
        colliders = Physics.OverlapSphere(meleePos.position, 3);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                //collider.GetComponent<PlayerHealth>().UpdateHealth(-damage);
                Debug.Log("Hit Player");
            }
        }
        StartCoroutine(MeleeAttackCooldown());
    }

    //void OnDrawGizmosSelected()
    //{

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(meleePos.position, 3);
    //}

    IEnumerator MeleeAttackCooldown()
    {
        yield return new WaitForSeconds(meleeCooldown);
        canPerformMeleeAttack = true;
    }
    IEnumerator MeleeAttackPerformTime()
    {
        yield return new WaitForSeconds(meleePerformTime);
        isAttacking = false;
    }
}
