using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public delegate void BarrelDestroyEvent();
    public BarrelDestroyEvent OnDestroyed;
    [SerializeField] private int health;
    private bool exploded = false;

    [SerializeField]private float timer = 6.0f;


    [SerializeField] private float explosionRadius = 5.0f;
    [SerializeField] private float explosionForce = 60.0f;

    public AudioSource source;
    public AudioClip barrelSound;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        source = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioSource>();
        source.volume = 0.3f;
    }

    private void Explode()
    {
 
       
        Collider[] objectsInExplosion = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var objectToDamage in objectsInExplosion)
        {
            var rb = objectToDamage.GetComponent<Rigidbody>();
            if (rb == null) continue;

            //Check if this object is an enemy. If so: damage and afflict gas
           if(objectToDamage.TryGetComponent(out EnemyStats e))
            {
                e.Afflict(MatterState.Gas, 4.0f);
                e.TakeDamage(25.0f);
            }

           //Check if object in the radius is a player, if so: damage
           if(objectToDamage.TryGetComponent(out PlayerStats p))
            {
                p.hp -= 1;
            }

            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            source.PlayOneShot(barrelSound);

            exploded = true;
            this.gameObject.SetActive(false);
            OnDestroyed?.Invoke();
        }
    }

    public void DamageBarrel()
    {

        if (health > 0)
        {

            health -= 10;
        }
        else if (!exploded)
        {
            StartCoroutine(StartExplosion(timer));
            
        }
    }

    IEnumerator StartExplosion(float seconds)
    {
        Debug.Log("Timer Started");
        yield return new WaitForSeconds(seconds);
        Debug.Log("Timer Ended");
        Explode();

    }
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public bool Exploded
    {
        get { return exploded; }
        set { exploded = value; }
    }
}
