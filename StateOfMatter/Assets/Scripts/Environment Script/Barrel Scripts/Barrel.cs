using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public delegate void BarrelDestroyEvent();
    public BarrelDestroyEvent OnDestroyed;
    public int health;
    public bool exploded = false;

    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 60f;

    public AudioSource source;
    public AudioClip barrelSound;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        source = gameObject.AddComponent<AudioSource>();
        source.volume = 0.3f;
    }

    private void Explode()
    {
        Collider[] objectsInExplosion = Physics.OverlapSphere(transform.position, explosionRadius);
      
        foreach(var objectToDamage in objectsInExplosion)
        {
            var rb = objectToDamage.GetComponent<Rigidbody>();
            if (rb == null) continue;

            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            source.PlayOneShot(barrelSound);
        }
    }

    public void DamageBarrel()
    {

        if (health > 0)
        {

            health -= 10;
            Debug.Log(health);
        }
        else if(!exploded)
        {
            Explode();
            exploded = true;
            this.gameObject.SetActive(false);
            OnDestroyed?.Invoke();
        }
    }
}
