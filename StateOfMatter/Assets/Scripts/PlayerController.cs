using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Instance Editable variables
    public float maxWalkSpeed = 5.0f;
    public float movementAccelGround = 1000.0f;
    public float movementAccelAir = 100.0f;
    public float movementDrag = 0.5f;
    public float jumpHeight = 200.0f;
    public float sensitivity = 10.0f;
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
        isGrounded = CheckGroundedAndSlope(out RaycastHit slopeHit, out bool isSlopeWall);
        body.useGravity = !isGrounded; // so player isn't sliding down a slope

        // -- CAMERA CONTROL -- \\

        camPitch += Input.GetAxisRaw("Mouse X") * sensitivity;
        camRotQuat.eulerAngles = new(0, camPitch, 0);
        body.MoveRotation(camRotQuat.normalized); // camera pitch (also character transform pitch)
        camTransform.Rotate(camTransform.right, -Input.GetAxisRaw("Mouse Y") * sensitivity, Space.World); // camera yaw

        // -- BASIC MOVEMENT HANDLING -- \\

        Vector3 movementForce = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movementForce += transform.forward;
        if (Input.GetKey(KeyCode.S)) movementForce -= transform.forward;
        if (Input.GetKey(KeyCode.A)) movementForce -= transform.right;
        if (Input.GetKey(KeyCode.D)) movementForce += transform.right;
        if (!isSlopeWall) movementForce = Vector3.ProjectOnPlane(movementForce, slopeHit.normal).normalized;

        // Accelerate the player in their movement direction and apply ground drag force
        body.AddForce((isGrounded ? movementAccelGround : movementAccelAir) * movementForce);
        ApplyGroundDrag(slopeHit);
        ClampHorizSpeed(slopeHit, isSlopeWall);

        // -- JUMP -- \\

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Reset vertical vel
            body.velocity = new(body.velocity.x, 0, body.velocity.z);
            body.AddForce(transform.up * jumpHeight);
        }
    }

    // Check if the player is currently on a floor/ramp, update values accodingly
    private bool CheckGroundedAndSlope(out RaycastHit hit, out bool isSlopeWall)
    {
        // Check if the player is in the air or in an incline that is too steep
        if (Physics.Raycast(
            transform.position,                              // start at center of player
            Vector3.down,                                    // Trace down
            out hit,                                         // Store the hit info temporarily
            GetComponent<CapsuleCollider>().height * 0.6f)) // Trace down by slightly over half player height
        {
            // Is the angle between the player and surface greater than maxIncline?
            if (Vector3.Dot(Vector3.down, hit.normal) <= -Mathf.Cos(maxIncline * Mathf.Deg2Rad))
            {
                isSlopeWall = false;
                return true;
            }
            else isSlopeWall = true; // Too steep, so this ramp should be considered a wall
            return false;
        }
        isSlopeWall = false;
        return false;
    }

    // Helper function to calculate artificial drag that only acts while on ground
    private void ApplyGroundDrag(RaycastHit slopeHit)
    {
        if (isGrounded)
        {
            Vector3 velProjected = Vector3.ProjectOnPlane(body.velocity, slopeHit.normal);
            // this goes in opposite direction of velocity
            Vector3 dragForce = -movementDrag * velProjected.normalized;
            //Vector3 dragForce = velOnXZ * -movementDrag;
            if (velProjected.sqrMagnitude >= 0.25f)
                body.AddForce(dragForce);
            else
                body.AddForce(-velProjected * movementDrag);
        }
    }

    // Helper function to clamp horizontal speed
    private void ClampHorizSpeed(RaycastHit hit, bool isSlopeWall)
    {
        Vector3 velFlat = new(body.velocity.x, 0, body.velocity.z);
        velFlat = Mathf.Clamp(velFlat.magnitude, 0, maxWalkSpeed) * velFlat.normalized;
        velFlat.y = body.velocity.y;
        body.velocity = velFlat;
        // Keep player from bouncing on a ramp when going up
        if (isGrounded && !isSlopeWall)
            body.AddForce(-hit.normal * 10);
    }
}
