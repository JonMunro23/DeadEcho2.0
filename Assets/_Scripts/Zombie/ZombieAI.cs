using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class ZombieAI : MonoBehaviour
{
    ZombieHealth zombieHealth;
    public NavMeshAgent agent;
    Animator animator;
    [SerializeField]
    Transform meleePos, playerPosition;
    public float speed, meleeCooldown, meleePerformTime, meleeRange;
    public float distanceToPerformMeleeAttack;
    [SerializeField] float minTimeBetweenGrowls, maxTimeBetweenGrowls;
    public int damage;
    public bool isDead, isMoving, isAttacking, canPerformMeleeAttack = true, seekPlayer = true, canGrowl;

    AudioSource zombieAudioSource;
    [SerializeField]
    AudioClip[] zombieGrowlAudioClips, zombieAttackingAudioClips;


    private void OnEnable()
    {
        PlayerHealth.onDeath += SetIdle;
    }

    private void OnDisable()
    {
        PlayerHealth.onDeath -= SetIdle;
    }

    private void Awake()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        zombieHealth = GetComponent<ZombieHealth>();
        animator = GetComponent<Animator>();
        zombieAudioSource = GetComponent<AudioSource>();
        StartCoroutine(GrowlCooldown());
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
        if (zombieHealth.isDead)
            return;

        if (canGrowl)
            Growl();

        if (seekPlayer)
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
        if(agent)
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
        animator.SetTrigger("MeleeAttack");
        PlayAttackAudio();
        StartCoroutine(MeleeAttackCooldown());
    }

    void CheckForHit()
    {
        Collider[] colliders;
        colliders = Physics.OverlapSphere(meleePos.position, meleeRange);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerHealth>().TakeDamage(damage);
                Debug.Log("Hit Player");
            }
        }
    }

    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(meleePos.position, meleeRange);
    }

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

    void SetIdle()
    {
        seekPlayer = false;
        HaltMovement();
    }

    void Growl()
    {
        Debug.Log("growling");
        canGrowl = false;
        int rand = Random.Range(0, zombieGrowlAudioClips.Length);

        zombieAudioSource.clip = zombieGrowlAudioClips[rand];
        zombieAudioSource.Play();
        StartCoroutine(GrowlCooldown());
    }

    void PlayAttackAudio()
    {
        Debug.Log("Playing attack audio");

        int rand = Random.Range(0, zombieAttackingAudioClips.Length);

        zombieAudioSource.clip = zombieAttackingAudioClips[rand];
        zombieAudioSource.Play();
    }

    IEnumerator GrowlCooldown()
    {
        float cooldownTime = Random.Range(minTimeBetweenGrowls, maxTimeBetweenGrowls);
        yield return new WaitForSeconds(cooldownTime);
        canGrowl = true;
    }
}
