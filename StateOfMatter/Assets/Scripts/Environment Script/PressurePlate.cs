using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PressurePlate : MonoBehaviour
{
    
    bool pressed = false;


    private void OnTriggerEnter(Collider other)
    {
        pressed = true;
        Debug.Log(pressed + " On Plate");

    }

    
    private void OnTriggerExit(Collider other)
    {
        pressed = false;
        Debug.Log(pressed + " Off Plate");
    }

    public bool Pressed
    {
        get { return pressed; }
        set { pressed = value; }
    }
}
