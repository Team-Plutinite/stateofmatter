using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenActive : MonoBehaviour, IActivatable
{
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Activate()
    {
        Debug.Log("Click");
       
    }

    public void Deactivate()
    {
        Debug.Log("Click");
    }
}