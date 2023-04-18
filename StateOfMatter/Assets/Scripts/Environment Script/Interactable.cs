using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public abstract void Activate();
    public abstract void Deactivate();
}

public interface IActivatable
{
    public abstract void Activate();
    public abstract void Deactivate();
}

public class Interactable : MonoBehaviour
{
    public GameObject objectToActivate;
    //public Component interactableComponent;
    
    public bool isInteractable;
    public bool isActivated;

    //public for debug purposes
    public bool isInRange;

    private IInteractable interactableScript;
    private IActivatable activatableScript;
    [SerializeField]
    public InteractRadius interactRadius;

    //private Transform[] allChildren;

    // Start is called before the first frame update
    void Start()
    {
        isInRange = false;
        //isActivated = false;
        //interactableScript = (IInteractable)interactableComponent;

        if ((IInteractable)gameObject.GetComponent<IInteractable>() != null)
        {
            interactableScript = (IInteractable)gameObject.GetComponent<IInteractable>();
            Debug.Log("Interactable script found on: " + gameObject.name);
        } else
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if ((IInteractable)child.gameObject.GetComponent<IInteractable>() != null)
                {
                    interactableScript = (IInteractable)child.gameObject.GetComponent<IInteractable>();
                    Debug.Log("Interactable script found on: " + child.gameObject.name);
                }
            }
        }
        
        if (interactableScript == null)
        {
            Debug.Log("No interactable scripts found on " + gameObject.name);
        }

        if (interactableScript != null)
        {
            //Debug.Log("Interactable script attached to " + gameObject.name);
        }

        interactRadius.InteractableEnter += InteractableInRange;
        interactRadius.InteractableExit += InteractableOutOfRange;
        if (objectToActivate != null)
        {
            activatableScript = (IActivatable)objectToActivate.GetComponent(typeof(IActivatable));
        }
    }


    private void OnValidate()
    {
        /*
        // checks if the component has the interactable interface. if it does, cast it to it
        if (!(interactableComponent is IInteractable))
        {
            interactableComponent = null;
        }
        if (interactableComponent != null)
        {
            interactableScript = (IInteractable)interactableComponent;
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKey(KeyCode.E))
        {
            Debug.Log("attempted interaction");
        }
        */
        if (isInteractable && isInRange && Input.GetKeyDown(KeyCode.E))
        {
            isActivated = true;
        }
        if (isActivated)
        {
            interactableScript.Activate();
            activatableScript.Activate();
            isActivated = false;
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

    public void Reset()
    {
        interactableScript.Deactivate();
    }

}
