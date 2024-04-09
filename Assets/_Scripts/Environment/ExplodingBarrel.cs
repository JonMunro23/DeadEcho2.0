using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour
{
    [SerializeField] float fuseTime, blastRadius;
    [SerializeField] int damage;
    [SerializeField] int currentHealth, maxHealth;

    [SerializeField] GameObject explosionEffect;
    GameObject onFireEffect;

    AudioSource audioSource;
    Coroutine explosionTimer;

    private void Awake()
    {
        onFireEffect = transform.GetChild(1).gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, blastRadius);
    //}

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            if (explosionTimer == null)
                explosionTimer = StartCoroutine(ExplosionTimer());
            else
            {
                StopCoroutine(explosionTimer);
                Explode();
            }
        }
    }

    void Explode()
    {
        onFireEffect.SetActive(false);
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("ZombieBody"))
            {
                float proximity = (transform.position - collider.transform.position).magnitude;
                float effect = 1 - (proximity / blastRadius);
                int damageTaken = Mathf.RoundToInt(damage * effect);
                collider.GetComponentInParent<ZombieHealth>().TakeDamage(damageTaken, collider.tag);
            }
            //else if (collider.CompareTag("ExplodingBarrel"))
            //{
            //    collider.GetComponent<ExplodingBarrel>().TakeDamage(damage);
            //}
            //if (collider.CompareTag("Player"))
            //{
            //    collider.GetComponent<PlayerHealth>().removehealth(damage);
            //}
        }
        var clone = Instantiate(explosionEffect, gameObject.transform.position, Quaternion.identity);
        Destroy(clone.gameObject, 2);
        Destroy(gameObject);
    }

    IEnumerator ExplosionTimer()
    {
        onFireEffect.SetActive(true);
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }
}
