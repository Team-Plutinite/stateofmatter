using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class WeaponAttackRadius : MonoBehaviour
{

    public delegate void MeltableEnteredEvent(Meltable melting);
    public delegate void MeltableExitedEvent(Meltable melting);

    public MeltableEnteredEvent MeltEnter;
    public MeltableExitedEvent MeltExit;

    public delegate void EnemyEnteredEvent(EnemyStats enemy);
    public delegate void EnemyExitedEvent(EnemyStats enemy);

    public EnemyEnteredEvent OnStay;
    public EnemyEnteredEvent OnExit;

    public delegate void InteractableEnteredEvent(Interactable interactable);
    public delegate void InteractableExitEvent(Interactable interactable);

    public InteractableEnteredEvent InteractableEnter;
    public InteractableExitEvent InteractableExit;

    private List<EnemyStats> EnemiesInRadius = new List<EnemyStats>();
    private List<Meltable> MeltablesInRadius = new List<Meltable>();
    private List<Interactable> InteractablesInRadius = new List<Interactable>();


    private void OnTriggerStay(Collider other)
    {
       //This function will handle collison with various objects.
       if (other.TryGetComponent<EnemyStats>(out EnemyStats enemy))
       {
            EnemiesInRadius.Add(enemy);
            OnStay?.Invoke(enemy);
        
       }

        if (other.TryGetComponent<Meltable>(out Meltable ice))
        {
            MeltablesInRadius.Add(ice);
            MeltEnter?.Invoke(ice);

        }

        if (other.TryGetComponent<Interactable>(out Interactable interactable))
        {
            InteractablesInRadius.Add(interactable);
            InteractableEnter?.Invoke(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<EnemyStats>(out EnemyStats enemy))
        {
            EnemiesInRadius.Remove(enemy);
            OnExit?.Invoke(enemy);

        }

        if (other.TryGetComponent<Meltable>(out Meltable ice))
        {
            MeltablesInRadius.Remove(ice);
            MeltExit?.Invoke(ice);

        }

        if (other.TryGetComponent<Interactable>(out Interactable interactable))
        {
            InteractablesInRadius.Remove(interactable);
            InteractableExit?.Invoke(interactable);
        }
    }
}
