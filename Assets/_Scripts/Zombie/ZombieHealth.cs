using UnityEngine;
using System;
using System.Collections;

public class ZombieHealth : MonoBehaviour, IDamageable
{
    ZombieAI zombieAI;

    [SerializeField] int hitPoints, baseKillPoints, extraHeadshotPoints;
    [SerializeField] SkinnedMeshRenderer zombieHead;

    //[SerializeField] GameObject zombieRagdoll;

    public int currentHealth;
    public bool isDead;

    [SerializeField] Rigidbody[] rigidBodyParts;

    [Header("Sound")]
    [SerializeField] bool canPlayHitSFX;
    [SerializeField] float hitSFxCooldown;
    public AudioClip[] headHitSFx;
    public AudioClip[] shoulderHitSFx;
    public AudioClip[] legHitSFx;
    AudioSource gettngHitAudioSource;

    [Header("Animation")]
    public float hitAnimCooldown;
    bool canPlayHitResponseAnim;
    Animator animator;

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
        SetRigidbodyActive(false);
        SetHealth(100);
        canPlayHitSFX = true;
        canPlayHitResponseAnim = true;
    }

    void SetRigidbodyActive(bool isActive)
    {
        foreach (Rigidbody rb in rigidBodyParts)
        {
            if(isActive)
                rb.gameObject.layer = LayerMask.NameToLayer("ZombieRagdoll");


            rb.useGravity = isActive;
            rb.isKinematic = !isActive;
        }
    }

    public void TakeDamage(int healthToRemove, string bodyPartTag)
    {
        if(!isDead)
        {
            if(canPlayHitResponseAnim)
                PlayHitResponse(bodyPartTag);

            if(canPlayHitSFX)
                PlayHitAudio(bodyPartTag);

            currentHealth -= healthToRemove;
            if (currentHealth <= 0)
            {
                if(bodyPartTag == "ZombieHead")
                {
                    ExplodeHead(false);
                    return;
                }

                Die(bodyPartTag == "ZombieHead");
                return;
            }

            onPointsGiven?.Invoke(hitPoints);
            onHit?.Invoke();
        }
    }

    void PlayHitResponse(string bodyPartTag)
    {
        canPlayHitResponseAnim = false;
        StartCoroutine(HitAnimResponseCooldown());
        switch (bodyPartTag)
        {
            case "ZombieHead":
                animator.Play("Hit-Head", 1);
                break;
            case "ZombieBody":
                int rand = UnityEngine.Random.Range(0, 2);
                if(rand == 1)
                    animator.Play("Hit-LeftShoulder", 1);
                else
                    animator.Play("Hit-RightShoulder", 1);
                break;
            case "ZombieLeftArm":
                animator.Play("Hit-LeftShoulder", 1);
                break;
            case "ZombieRightArm":
                animator.Play("Hit-RightShoulder", 1);
                break;
            //case "ZombieLeftLeg":
            //    animator.Play("Hit-LeftLeg", 0);
            //    break;
            //case "ZombieRightLeg":
            //    animator.Play("Hit-RightLeg", 0);
            //    break;
        }
    }

    void PlayHitAudio(string bodyPartTag)
    {
        canPlayHitSFX = false;
        StartCoroutine(HitSFXCooldown());
        switch (bodyPartTag)
        {
            case "ZombieHead":
                gettngHitAudioSource.PlayOneShot(GetRandomAudioClipFromArray(headHitSFx));
                break;
            case "ZombieBody":
                gettngHitAudioSource.PlayOneShot(GetRandomAudioClipFromArray(shoulderHitSFx));
                break;
            case "ZombieLeftArm":
                gettngHitAudioSource.PlayOneShot(GetRandomAudioClipFromArray(shoulderHitSFx));
                break;
            case "ZombieRightArm":
                gettngHitAudioSource.PlayOneShot(GetRandomAudioClipFromArray(shoulderHitSFx));
                break;
            case "ZombieLeftLeg":
                gettngHitAudioSource.PlayOneShot(GetRandomAudioClipFromArray(legHitSFx));
                break;
            case "ZombieRightLeg":
                gettngHitAudioSource.PlayOneShot(GetRandomAudioClipFromArray(legHitSFx));
                break;
        }
    }

    AudioClip GetRandomAudioClipFromArray(AudioClip[] clipArray)
    {
        int rand = UnityEngine.Random.Range(0, clipArray.Length);
        return clipArray[rand];
    }

    public void ExplodeHead(bool wasInstantlyKilled)
    {
        zombieHead.enabled = false;
        if (!isDead)
        {
            if(!wasInstantlyKilled)
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

        SetRigidbodyActive(true);
        animator.enabled = false;
        zombieAI.agent.enabled = false;

        //foreach(Rigidbody rb in rigidBodyParts)
        //{
        //    rb.AddForce(-transform.forward * 4, ForceMode.Impulse);
        //}

        //animator.SetBool("IsMoving", false);
        //animator.SetBool("IsDead", isDead);
        //transform.SetParent(null);
        //Collider[] colliders = GetComponentsInChildren<Collider>();
        //foreach (Collider collider in colliders)
        //{
        //    collider.enabled = false;
        //}    

        //Destroy(zombieAI.agent);
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

    public void OnDamaged(int damageTaken, string bodyPartTag)
    {
        TakeDamage(damageTaken, bodyPartTag);
    }

    public void InstantlyKill()
    {
        ExplodeHead(true);
    }

    IEnumerator HitSFXCooldown()
    {
        yield return new WaitForSeconds(hitSFxCooldown);
        canPlayHitSFX = true;
    }

    IEnumerator HitAnimResponseCooldown()
    {
        yield return new WaitForSeconds(hitAnimCooldown);
        canPlayHitResponseAnim = true;
    }
}
