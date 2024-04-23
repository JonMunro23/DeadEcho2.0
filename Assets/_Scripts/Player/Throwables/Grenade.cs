using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float grenadeFuseTime, blastRadius;
    [SerializeField] int damage;

    [SerializeField] bool explodesOnContact, hasTimer;

    [SerializeField] GameObject explosionEffect;

    AudioSource audioSource;

    bool hitSomething;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(hasTimer)
            StartCoroutine(GrenadeFuseTimer());
    }

    IEnumerator GrenadeFuseTimer()
    {
        yield return new WaitForSeconds(grenadeFuseTime);
        Explode();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("ZombieBody"))
            {
                //hitSomething = true;
                float proximity = (transform.position - collider.transform.position).magnitude;
                float effect = 1 - (proximity / blastRadius);
                int damageTaken = Mathf.RoundToInt(damage * effect);
                collider.GetComponentInParent<ZombieHealth>().TakeDamage(damageTaken, collider.tag);
            }
            //if (collider.CompareTag("Player"))
            //{
            //    collider.GetComponent<PlayerHealth>().removehealth(damage);
            //}
        }
            
        var clone = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
        Destroy(clone.gameObject, 2);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if(!other.CompareTag("Player") && !other.CompareTag("PlayerBody") && !other.CompareTag("PlayerSpawnRadius") && !other.CompareTag("ZombieSpawnPoint"))
        {
            if (explodesOnContact)
                Explode();
            else
                audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
