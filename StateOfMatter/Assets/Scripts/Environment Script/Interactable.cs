using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public abstract void Activate();
}

public class Interactable : MonoBehaviour
{
    public GameObject objectToActivate;
    [SerializeField]
    private WeaponAttackRadius attackRadius;
    public Component interactableComponent;
    protected IInteractable interactableScript;
    
    public bool isInRange;
    public bool isInteractable;
    public bool isActivated;

    // Start is called before the first frame update
    void Start()
    {
        isInRange = false;
        isInteractable = true;
        isActivated = false;

        attackRadius.InteractableEnter += InteractableInRange;
        attackRadius.InteractableExit += InteractableOutOfRange;
    }


    private void OnValidate()
    {
        // checks if the component has the interactable interface. if it does, cast it to it
        if (!(interactableComponent is IInteractable))
        {
            interactableComponent = null;
        }
        if (interactableComponent != null)
        {
            interactableScript = (IInteractable)interactableComponent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInteractable && isInRange)
        {
            interactableScript.Activate();
            // objectToActivate.Activate();
            isActivated = true;
        }
    }

    private void InteractableInRange(Interactable interactable)
    {
        interactable.isInRange = true;
    }

    private void InteractableOutOfRange(Interactable interactable)
    {
        interactable.isInRange = false;
    }

}
