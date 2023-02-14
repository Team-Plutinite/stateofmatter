using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.Find("Player"))
        {
            Rigidbody t = GameObject.Find("Player").GetComponent<Rigidbody>();
            Debug.Log("asd");
            t.velocity = new(t.velocity.x, 0, t.velocity.z);
        }
    }
}
