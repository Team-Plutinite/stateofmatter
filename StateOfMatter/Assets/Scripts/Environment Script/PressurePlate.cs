using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PressurePlate : MonoBehaviour
{
    [SerializeField]
    Material m;
    

    private bool pressed = false;


    private void OnTriggerEnter(Collider other)
    {
        this.pressed = true;
        Debug.Log(pressed + " On Plate");
        m.SetColor("_Color", Color.green);

    }

    
    private void OnTriggerExit(Collider other)
    {
        this.pressed = false;
        Debug.Log(pressed + " Off Plate");
        m.SetColor("_Color", Color.red);
    }

    public bool Pressed
    {
        get { return pressed; }
        set { pressed = value; }
    }
}
