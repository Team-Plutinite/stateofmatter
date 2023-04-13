using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class GasAttacker : MonoBehaviour
{
    public delegate void MeltableEnteredEvent(Meltable melting);
    public delegate void MeltableExitedEvent(Meltable melting);

    public MeltableEnteredEvent MeltEnter;
    public MeltableExitedEvent MeltExit;

    public delegate void EnemyEnteredEvent(EnemyStats enemy, GameObject cloud);
    public delegate void EnemyExitedEvent(EnemyStats enemy, GameObject cloud);

    public EnemyEnteredEvent OnStay;
    public EnemyEnteredEvent OnExit;

    // Track all enemies that have come into contact with this cloud
    private List<EnemyStats> EnemiesHit = new List<EnemyStats>();
    private List<Meltable> MeltablesInRadius = new List<Meltable>();

    private float lifetime;
    private float lifetimeMax;

    // Should the cloud knock enemies back upon contact?
    private bool knockbackable;

    private void Update()
    {
        // Set inactive when lifetime runs out
        lifetime -= Time.deltaTime;
        // cloud knocks enemies back for the first 20% of its life
        knockbackable = lifetime >= lifetimeMax * 0.8f; 
        if (lifetime <= 0.0f && gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    // Spawns this gas cloud at the given location with the given rotation
    public void Spawn(Vector3 position, Quaternion rotation, float lifetime)
    {
        transform.SetPositionAndRotation(position, rotation);
        this.lifetime = lifetime;
        gameObject.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
       //This function will handle collison with various objects.
       if (other.TryGetComponent(out EnemyStats enemy))
       {
            // If this is the first time enemy has been hit by this cloud
            // and the cloud was recently created, knock the enemy back
            if (!EnemiesHit.Contains(enemy) && knockbackable)
            {
                EnemiesHit.Add(enemy);
                enemy.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 50f);
            }
            OnStay?.Invoke(enemy, this.gameObject);
       }

        if (other.TryGetComponent(out Meltable ice))
        {
            MeltablesInRadius.Add(ice);
            MeltEnter?.Invoke(ice);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out EnemyStats enemy))
        {
            OnExit?.Invoke(enemy, this.gameObject);
        }

        if (other.TryGetComponent(out Meltable ice))
        {
            MeltablesInRadius.Remove(ice);
            MeltExit?.Invoke(ice);
        }
    }
}
