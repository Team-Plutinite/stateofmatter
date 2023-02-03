using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Instance Editable variables
    public float maxWalkSpeed = 5.0f;
    public float movementAccel = 1000.0f;
    public float jumpHeight = 200.0f;
    public float sensitivity = 10.0f;
    public float movementDrag = 0.5f;

    [Tooltip("Set the max incline angle the player can walk up, in degrees")]
    public float maxIncline = 35;

    // Reference to player rigid body
    private Rigidbody body;

    // Camera variables
    private Transform camTransform;
    private float camPitch = 0;
    private Quaternion camRotQuat;

    // Raycast down to check if player is grounded
    [SerializeField]
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        camTransform = transform.GetChild(0);
        body = GetComponent<Rigidbody>();

        isGrounded = false;

        Cursor.lockState = CursorLockMode.Locked; // lock to middle of screen and set invisible
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player can jump
        isGrounded = Physics.Raycast(transform.position, Vector3.down, GetComponent<CapsuleCollider>().height*0.6f);

        Vector3 movementForce = Vector3.zero;

        // -- CAMERA CONTROL -- \\

        camPitch += Input.GetAxisRaw("Mouse X") * sensitivity;
        camRotQuat.eulerAngles = new(0, camPitch, 0);
        body.MoveRotation(camRotQuat.normalized); // camera pitch (also character transform pitch)
        camTransform.Rotate(camTransform.right, -Input.GetAxisRaw("Mouse Y") * sensitivity, Space.World); // camera yaw

        // -- BASIC MOVEMENT HANDLING -- \\

        if (Input.GetKey(KeyCode.W)) movementForce += transform.forward;
        if (Input.GetKey(KeyCode.S)) movementForce -= transform.forward;
        if (Input.GetKey(KeyCode.A)) movementForce -= transform.right;
        if (Input.GetKey(KeyCode.D)) movementForce += transform.right;
        movementForce.Normalize(); // Fix the diagonal speed thing

        // Artificial drag that only acts on X and Z (so gravity is not affected)

        Vector3 velOnXZ = body.velocity;
        velOnXZ.y = 0;
        // this goes in opposite direction of velocity
        Vector3 dragForce = -movementDrag * velOnXZ.normalized;
        if (velOnXZ.sqrMagnitude >= 0.25f)
            body.AddForce(dragForce);
        else
            body.AddForce(-velOnXZ * movementDrag);

        // -- JUMP -- \\

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Reset vertical vel
            body.velocity = new(body.velocity.x, 0, body.velocity.z);
            body.AddForce(transform.up * jumpHeight);
        }

        // Clamp horizontal speed
        Vector3 horizVel = body.velocity;
        float tempY = horizVel.y;
        horizVel.y = 0;
        horizVel = Mathf.Clamp(horizVel.magnitude, 0, maxWalkSpeed) * horizVel.normalized;
        horizVel.y = tempY;
        body.velocity = horizVel;

        // Accelerate the player in their movement direction
        body.AddForce(movementAccel * movementForce);
    }
}
