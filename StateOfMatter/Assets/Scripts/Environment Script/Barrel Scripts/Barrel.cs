using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    int health;
    bool exploded = false;

    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 60f;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
    }

    private void Explode()
    {
        Collider[] objectsInExplosion = Physics.OverlapSphere(transform.position, explosionRadius);
      
        foreach(var objectToDamage in objectsInExplosion)
        {
            var rb = objectToDamage.GetComponent<Rigidbody>();
            if (rb == null) continue;

            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
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
        }
    }
}
