using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public delegate void BarrelDestroyEvent();
    public BarrelDestroyEvent OnDestroyed;
    [SerializeField] private int health;
    private bool exploded = false;
    private bool timerOn = false;

    [SerializeField]private float timer = 6f;


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
        while(timerOn)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
                Debug.Log(timer);
            }
            else
            {
                timer = 0;
                timerOn = false;
            }
        }
        Collider[] objectsInExplosion = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var objectToDamage in objectsInExplosion)
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
        }
        else if (!exploded)
        {
            Debug.Log("Timer On");
            timerOn = true;   
            Explode();
            exploded = true;
            this.gameObject.SetActive(false);
            OnDestroyed?.Invoke();
        }
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
