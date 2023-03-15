using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Move();
        }

        if (Input.GetKey(KeyCode.Z))
        {
            this.gameObject.transform.Translate(new Vector3(-.01f, 0f, 0f));
        }
    }


    public void Move()
    {
        this.gameObject.transform.SetPositionAndRotation(new Vector3(5.8f, -0.04f, -1.507777f), new Quaternion());
    }
}
