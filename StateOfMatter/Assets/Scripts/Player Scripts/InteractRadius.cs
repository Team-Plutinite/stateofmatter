using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class InteractRadius : MonoBehaviour
{

    public delegate void InteractableEnteredEvent(Interactable interactable);
    public delegate void InteractableExitEvent(Interactable interactable);

    public InteractableEnteredEvent InteractableEnter;
    public InteractableExitEvent InteractableExit;

    private List<Interactable> InteractablesInRadius = new List<Interactable>();

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Interactable>(out Interactable interactable))
        {
            InteractablesInRadius.Add(interactable);
            InteractableEnter?.Invoke(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Interactable>(out Interactable interactable))
        {
            InteractablesInRadius.Remove(interactable);
            InteractableExit?.Invoke(interactable);
        }
    }
}
