using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class WeaponAttackRadius : MonoBehaviour
{

    public delegate void EnemyEnteredEvent(EnemyStats enemy);
    public delegate void EnemyExitedEvent(EnemyStats enemy);

    public delegate void MeltableEnteredEvent(Meltable melting);
    public delegate void MeltableExitedEvent(Meltable melting);

    public EnemyEnteredEvent OnEnter;
    public EnemyEnteredEvent OnExit;
    
    public MeltableEnteredEvent MeltEnter;
    public MeltableExitedEvent MeltExit;

    

    private List<EnemyStats> EnemiesInRadius = new List<EnemyStats>();
    private List<Meltable> MeltablesInRadius = new List<Meltable>();

    private void OnTriggerStay(Collider other)
    {
       //This function will handle collison with Enemies.
       if (other.TryGetComponent<EnemyStats>(out EnemyStats enemy))
       {
            EnemiesInRadius.Add(enemy);
            OnEnter?.Invoke(enemy);
            Debug.Log("Hi!");
       }

        //This function will handle collison with environmental ice.
        if (other.TryGetComponent<Meltable>(out Meltable melts))
        {
            MeltablesInRadius.Add(melts);
            MeltEnter?.Invoke(melts);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<EnemyStats>(out EnemyStats enemy))
        {
            EnemiesInRadius.Remove(enemy);
            OnExit?.Invoke(enemy);

        }

        if (other.TryGetComponent<Meltable>(out Meltable melts))
        {
            MeltablesInRadius.Remove(melts);
            MeltExit?.Invoke(melts);

        }

    }


}
