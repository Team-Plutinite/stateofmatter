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

    public delegate void EnemyEnteredEvent(EnemyStats enemy);
    public delegate void EnemyExitedEvent(EnemyStats enemy);

    public EnemyEnteredEvent OnStay;
    public EnemyEnteredEvent OnExit;

    private List<EnemyStats> EnemiesInRadius = new List<EnemyStats>();
    private List<Meltable> MeltablesInRadius = new List<Meltable>();
    private List<Barrel> BarrelsInRadius = new List<Barrel>();

    private float lifetime;

    private void Update()
    {
        // Set inactive when lifetime runs out
        lifetime -= Time.deltaTime;
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
            EnemiesInRadius.Add(enemy);
            OnStay?.Invoke(enemy);
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
        if(other.TryGetComponent(out EnemyStats enemy))
        {
            EnemiesInRadius.Remove(enemy);
            OnExit?.Invoke(enemy);
        }

        if (other.TryGetComponent(out Meltable ice))
        {
            MeltablesInRadius.Remove(ice);
            MeltExit?.Invoke(ice);
        }

        if (other.TryGetComponent(out Barrel barrel))
        {
            BarrelsInRadius.Remove(barrel);
            BarrelEnter?.Invoke(barrel);
        }
    }
}
