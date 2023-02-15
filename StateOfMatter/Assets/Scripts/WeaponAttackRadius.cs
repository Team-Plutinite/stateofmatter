using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class WeaponAttackRadius : MonoBehaviour
{

    public delegate void EnemyEnteredEvent(Enemy enemy);
    public delegate void EnemyExitedEvent(Enemy enemy);

    public EnemyEnteredEvent OnEnter;
    public EnemyEnteredEvent OnExit;

    private List<Enemy> EnemiesInRadius = new List<Enemy>();


    private void OnTriggerEnter(Collider other)
    {
       //This function will handle collison with various objects.
       if (other.TryGetComponent<Enemy>(out Enemy enemy))
       {
            EnemiesInRadius.Add(enemy);
            OnEnter?.Invoke(enemy);
        
       }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            EnemiesInRadius.Remove(enemy);
            OnExit?.Invoke(enemy);

        }
    }
}
