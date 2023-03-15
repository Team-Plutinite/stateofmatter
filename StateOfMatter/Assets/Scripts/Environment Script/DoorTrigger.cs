using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    private List<PressurePlate> pressurePlates;

    bool IsOpen = false;

    private void Update()
    {
        if(pressurePlates.TrueForAll(pp => pp.Pressed) && !IsOpen)
        {
            OpenDoor();
            IsOpen = true;
        }
    }

    public void OpenDoor()
    {
        Debug.Log("Door Open");
    }
}
