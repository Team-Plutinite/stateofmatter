using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBeltController : MonoBehaviour
{
    [SerializeField] private GameObject[] axles;
    public float beltSpeed = 300.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rotation = new(Time.deltaTime * beltSpeed, 0, 0);
        foreach (GameObject axle in axles)
            axle.GetComponent<Rigidbody>().AddRelativeTorque(rotation);
    }
}
