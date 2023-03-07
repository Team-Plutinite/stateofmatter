using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class PressurePlate : MonoBehaviour
{
    [SerializeField]
    BoxCollider pressureCollider;

    public delegate void PressurePlateEntered(GameObject gO);
    public delegate void PressurePlateExited(GameObject gO);

    public PressurePlateEntered OnStay;
    public PressurePlateExited OnExit; 

    //Holds the current pressure plate value.
    bool depressed = false;

    private List<GameObject> ObjectOnPlate = new List<GameObject>();   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<GameObject>(out GameObject o))
        {
            ObjectOnPlate.Add(o);
            OnStay?.Invoke(o);
            depressed = true;
            Debug.Log(depressed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<GameObject>(out GameObject o))
        {
            ObjectOnPlate.Remove(o);
            OnStay?.Invoke(o);
            depressed = false;
            Debug.Log(depressed);
        }
    }

    public bool Depressed
    {
        set { depressed = value; }
        get { return depressed; }
    }
}
