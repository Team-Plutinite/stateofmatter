using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    private List<PressurePlate> pressurePlates;

    private void Update()
    {
        if(pressurePlates.TrueForAll(pp => pp.Pressed))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        Debug.Log("Door Open");
    }
}
