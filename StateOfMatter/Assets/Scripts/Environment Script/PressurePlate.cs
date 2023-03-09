using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PressurePlate : MonoBehaviour
{
    private bool pressed = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            this.pressed = true;
            Debug.Log(pressed + " On Plate");
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {
            this.pressed = false;
            Debug.Log(pressed + " Off Plate");            
        }

    }

    public bool Pressed
    {
        get { return pressed; }
        set { pressed = value; }
    }
}
