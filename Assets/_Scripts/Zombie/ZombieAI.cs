using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class ZombieAI : MonoBehaviour
{
    ZombieHealth zombieHealth;
    public NavMeshAgent agent;
    Animator animator;
    [SerializeField]
    Transform meleePos, playerPosition;
    public float speed, meleeCooldown, meleePerformTime;
    public float distanceToPerformMeleeAttack;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!zombieHealth.isDead)
        {
            SetTarget();    

            if (Vector3.Distance(transform.position, playerPosition.position) > distanceToPerformMeleeAttack)
            {
                if (!isMoving)
                {
                    MoveTowardsTarget();
                }
            }
            else if(Vector3.Distance(transform.position, playerPosition.position) <= distanceToPerformMeleeAttack)
            {
                if (isMoving)
                    HaltMovement();

                LookAtTarget(agent.destination);

                if (canPerformMeleeAttack)
                    MeleeAttack();
            }
        }
    }

    void HaltMovement()
    {
        isMoving = false;
        animator.SetBool("IsMoving", isMoving);
        agent.isStopped = true;
    }

    void MoveTowardsTarget()
    {
        isMoving = true;
        animator.SetBool("IsMoving", isMoving);
        agent.isStopped = false;
    }

    private void SetTarget()
    {
        agent.SetDestination(playerPosition.position);
    }

    void MeleeAttack()
    {
        canPerformMeleeAttack = false;
        Collider[] colliders;
        animator.SetTrigger("MeleeAttack");
        colliders = Physics.OverlapSphere(meleePos.position, 3);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerHealth>().TakeDamage(damage);
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

    void LookAtTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.LookRotation(dir), agent.angularSpeed * Time.deltaTime);
    }
}
