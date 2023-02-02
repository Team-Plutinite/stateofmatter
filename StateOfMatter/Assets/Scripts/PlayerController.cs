using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 1.0f;
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
        Vector3 velocity = Vector3.zero;

        // -- BASIC MOVEMENT HANDLING -- \\
        if (Input.GetKey(KeyCode.W)) velocity += camTransform.forward;
        if (Input.GetKey(KeyCode.S)) velocity -= camTransform.forward;
        if (Input.GetKey(KeyCode.A)) velocity -= camTransform.right;
        if (Input.GetKey(KeyCode.D)) velocity += camTransform.right;
        velocity.Normalize(); // Fix the diagonal speed thing

        // -- CAMERA CONTROL -- \\

        transform.Rotate(transform.up, Input.GetAxisRaw("Mouse X") * sensitivity, Space.World);
        camTransform.Rotate(camTransform.right, -Input.GetAxisRaw("Mouse Y") * sensitivity, Space.World);


        // Output resulting velocity
        GetComponent<Rigidbody>().AddForce(velocity * walkSpeed * Time.deltaTime);
    }


}
