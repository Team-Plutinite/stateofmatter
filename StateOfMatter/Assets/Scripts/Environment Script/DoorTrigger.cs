using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    private List<PressurePlate> pressurePlates;

    private IActivatable activatableScript;

    bool IsOpen = false;

    private void Start()
    {
        try
        {
            activatableScript = gameObject.GetComponent<IActivatable>();
        }
        catch
        {
            Debug.Log("No activatable script on object to activate");
        }
    }

    private void Update()
    {
        if(pressurePlates.TrueForAll(pp => pp.Pressed) && !IsOpen)
        {
            OpenDoor();
            IsOpen = true;
        }
    }

    // This can probably be condensed into just being in the Update method?
    public void OpenDoor()
    {
        activatableScript.Activate();
        Debug.Log("Door Open");
    }
}
