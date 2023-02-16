using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class WeaponAttackRadius : MonoBehaviour
{

    public delegate void EnemyEnteredEvent(EnemyStats enemy);
    public delegate void EnemyExitedEvent(EnemyStats enemy);

    public EnemyEnteredEvent OnEnter;
    public EnemyEnteredEvent OnExit;

    private List<EnemyStats> EnemiesInRadius = new List<EnemyStats>();


    private void OnTriggerEnter(Collider other)
    {
       //This function will handle collison with various objects.
       if (other.TryGetComponent<EnemyStats>(out EnemyStats enemy))
       {
            EnemiesInRadius.Add(enemy);
            OnEnter?.Invoke(enemy);
        
       }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<EnemyStats>(out EnemyStats enemy))
        {
            EnemiesInRadius.Remove(enemy);
            OnExit?.Invoke(enemy);

        }
    }
}
