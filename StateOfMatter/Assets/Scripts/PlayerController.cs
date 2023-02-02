using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 1.0f;
    public float jumpHeight = 2.0f;
    public float sensitivity = 10.0f;

    private Vector3 mouseVel, mousePosCur, mousePosPrev;

    private Transform camTransform;

    // Start is called before the first frame update
    void Start()
    {
        mouseVel = Vector3.zero;
        mousePosCur = Vector3.zero;
        mousePosPrev = Vector3.zero;
        camTransform = transform.GetChild(0);

        Cursor.lockState = CursorLockMode.Locked; // lock to middle of screen and set invisible
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movementForce = Vector3.zero;

        // -- CAMERA CONTROL -- \\
        transform.Rotate(transform.up, Input.GetAxisRaw("Mouse X") * sensitivity, Space.World);
        camTransform.Rotate(camTransform.right, -Input.GetAxisRaw("Mouse Y") * sensitivity, Space.World);

        // -- BASIC MOVEMENT HANDLING -- \\
        if (Input.GetKey(KeyCode.W)) movementForce += transform.forward * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) movementForce -= transform.forward * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) movementForce -= transform.right * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) movementForce += transform.right * Time.deltaTime;
        movementForce.Normalize(); // Fix the diagonal speed thing

        // -- JUMP -- \\
        if (Input.GetKeyDown(KeyCode.Space)) movementForce += transform.up * jumpHeight;

        // if not moving around, slow the player down
        Vector3 movementXZ = new Vector3(movementForce.x, movementForce.y);
        if (movementXZ.sqrMagnitude == 0)
        {
            Vector3 stop = -(GetComponent<Rigidbody>().velocity * 5);
            stop.y = 0;
            GetComponent<Rigidbody>().AddForce(stop);
        }

        // Output resulting velocity
        GetComponent<Rigidbody>().AddForce(walkSpeed * movementForce);
    }


}
