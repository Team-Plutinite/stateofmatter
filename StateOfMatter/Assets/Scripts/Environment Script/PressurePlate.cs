using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PressurePlate : MonoBehaviour
{
    [SerializeField]
    bool pressed = false;

    // Start is called before the first frame update
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

}
