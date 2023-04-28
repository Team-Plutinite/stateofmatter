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

    public delegate void BarrelEnteredEvent(Barrel barrel);
    public delegate void BarrelEnteredExit(Barrel barrel);

    public BarrelEnteredEvent BarrelEnter;
    public BarrelEnteredExit BarrelExit;

    public delegate void EnemyEnteredEvent(EnemyStats enemy, GameObject cloud);
    public delegate void EnemyExitedEvent(EnemyStats enemy, GameObject cloud);


    public EnemyEnteredEvent OnStay;
    public EnemyEnteredEvent OnExit;

    private List<EnemyStats> EnemiesHit = new List<EnemyStats>();
    private List<Meltable> MeltablesInRadius = new List<Meltable>();
    private List<Barrel> BarrelsInRadius = new List<Barrel>();

    private float lifetime;
    private float lifetimeMax;

    // Should the cloud knock enemies back upon contact?
    private bool knockbackable;

    private void Update()
    {
        // Set inactive when lifetime runs out
        lifetime -= Time.deltaTime;
        // cloud knocks enemies back for the first 10% of its life
        knockbackable = lifetime >= lifetimeMax * 0.9f;

        // Clear enemies hit and deactivate cloud when it's done
        if (lifetime <= 0.0f && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            EnemiesHit.Clear();
        }
    }

    // Spawns this gas cloud at the given location with the given rotation
    public void Spawn(Vector3 position, Quaternion rotation, float lifetime)
    {
        transform.SetPositionAndRotation(position, rotation);
        this.lifetime = lifetime;
        this.lifetimeMax = lifetime;
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
                enemy.gameObject.GetComponent<Rigidbody>().AddForce(-transform.forward * 500f, ForceMode.Acceleration);
                enemy.TakeDamage(5.0f);
            }
            OnStay?.Invoke(enemy, gameObject);
        }

        if (other.TryGetComponent(out Meltable ice))
        {
            MeltablesInRadius.Add(ice);
            MeltEnter?.Invoke(ice);
        }

        if (other.TryGetComponent(out Barrel barrel))
        {
            BarrelsInRadius.Add(barrel);
            BarrelEnter?.Invoke(barrel);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.TryGetComponent(out EnemyStats enemy))
        {
            OnExit?.Invoke(enemy, this.gameObject);
        }

        if (other.TryGetComponent(out Meltable ice))
        {
            MeltablesInRadius.Remove(ice);
            MeltExit?.Invoke(ice);
        }

        if (other.TryGetComponent(out Barrel barrel))
        {
            BarrelsInRadius.Remove(barrel);
            BarrelExit?.Invoke(barrel);
        }
    }
}
