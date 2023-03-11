using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public abstract void Activate();
}

public interface IActivatable
{
    public abstract void Activate();
}

public class Interactable : MonoBehaviour
{
    public GameObject objectToActivate;
    public Component interactableComponent;
    
    public bool isInRange;
    public bool isInteractable;
    public bool isActivated;

    private IInteractable interactableScript;
    private IActivatable activatableScript;
    [SerializeField]
    private InteractRadius interactRadius;

    // Start is called before the first frame update
    void Start()
    {
        isInRange = false;
        isInteractable = false;
        isActivated = false;

        interactRadius.InteractableEnter += InteractableInRange;
        interactRadius.InteractableExit += InteractableOutOfRange;
        try
        {
            activatableScript = objectToActivate.GetComponent<IActivatable>();
        } catch 
        {
            Debug.Log("No activatable script on object to activate");
        }
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
        if (isInteractable && isInRange && Input.GetKey(KeyCode.E))
        {
            interactableScript.Activate();
            activatableScript.Activate();
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
