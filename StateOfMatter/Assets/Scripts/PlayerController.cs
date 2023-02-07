using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Instance Editable variables
    public float maxWalkSpeed = 5.0f;
    public float movementAccelGround = 20.0f;
    public float movementAccelAir = 2.0f;
    public float groundDrag = 10.0f;
    public float airDrag = 2.0f;
    public float jumpHeight = 300.0f;
    public float jumpCooldown = 0.25f;
    public float lookSensitivity = 2.5f;
    [Tooltip("Set the max incline angle the player can walk up, in degrees")]
    public float maxIncline = 47.5f;

    // Reference to player rigid body
    private Rigidbody body;

    // Camera variables
    private Transform camTransform;
    private float camPitch = 0, camYaw = 0;
    private Quaternion camRotQuat;

    // Raycast down to check if player is grounded
    [SerializeField]
    private bool isGrounded;
    [SerializeField]
    private bool isCrouched;
    private float jumpTime;

    // Start is called before the first frame update
    void Start()
    {
        camTransform = transform.GetChild(0);
        body = GetComponent<Rigidbody>();

        isGrounded = false;
        isCrouched = false;
        jumpTime = 0.0f;

        Cursor.lockState = CursorLockMode.Locked; // lock to middle of screen and set invisible
    }

    // Update is called once per frame
    void Update()
    {
        // -- CAMERA CONTROL -- \\

        camPitch += Input.GetAxisRaw("Mouse X") * lookSensitivity;
        camYaw = Mathf.Clamp(camYaw + (Input.GetAxisRaw("Mouse Y") * lookSensitivity), -90, 90);
        camRotQuat.eulerAngles = new(0, camPitch, 0);
        body.MoveRotation(camRotQuat.normalized); // camera pitch (also character transform pitch)
        camTransform.localRotation = Quaternion.Euler(-camYaw, 0, 0); // camera yaw

        // -- GROUND-ONLY MOVEMENT CONTROLS -- \\
        jumpTime -= Time.deltaTime; // jumping cooldown
        if (isGrounded)
        {
            // -- CROUCH/UNCROUCH -- \\
            if (Input.GetKey(KeyCode.LeftControl))
                TryCrouch();
            else
                TryUncrouch();

            // -- JUMP -- \\
            if (jumpTime <= 0 && Input.GetKeyDown(KeyCode.Space))
            {
                jumpTime = jumpCooldown;
                isGrounded = false;
                // Reset vertical vel
                body.velocity = new(body.velocity.x, 0, body.velocity.z);
                body.AddForce(transform.up * jumpHeight);
                TryUncrouch(); // uncrouch the player if they're crouching
            }
        }
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGroundedAndSlope(out RaycastHit slopeHit, out bool isSlopeWall);
        body.useGravity = !isGrounded; // so player isn't sliding down a slope

        // -- BASIC MOVEMENT HANDLING -- \\

        Vector3 movementForce = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movementForce += transform.forward;
        if (Input.GetKey(KeyCode.S)) movementForce -= transform.forward;
        if (Input.GetKey(KeyCode.A)) movementForce -= transform.right;
        if (Input.GetKey(KeyCode.D)) movementForce += transform.right;
        if (!isSlopeWall) movementForce = Vector3.ProjectOnPlane(movementForce, slopeHit.normal).normalized;

        // Accelerate the player in their movement direction and apply ground drag force
        body.AddForce((isGrounded ? movementAccelGround : movementAccelAir) * movementForce);
        ApplyDrag(slopeHit);
        ClampHorizSpeed(slopeHit, isSlopeWall);
    }

    // Attempt a crouch
    void TryCrouch()
    {
        if (!isCrouched)
        {
            isCrouched = true;
            GetComponent<CapsuleCollider>().height /= 2;
            body.AddForce(new(0, -250)); // push the player down so they aren't floating for a second
        }
    }

    // Uncrouch, if currently crouched
    void TryUncrouch()
    {
        if (isCrouched)
        {
            isCrouched = false;
            GetComponent<CapsuleCollider>().height *= 2;
        }
    }

    // Check if the player is currently on a floor/ramp, update values accodingly
    private bool CheckGroundedAndSlope(out RaycastHit hit, out bool isSlopeWall)
    {
        // Don't run this method if the player just jumped
        if (jumpTime > 0)
        {
            isSlopeWall = false;
            hit = new();
            return false;
        }

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
    // I COULD turn this whole thing into a giant one-liner, but for the sake of readability I'll leave it as is
    private void ApplyDrag(RaycastHit slopeHit)
    {
        // Use ground or air drag based on if player is in the air or not
        float dragIntensity = isGrounded ? groundDrag : airDrag;

        // Get velocity of player projected onto the surface walked on (or the XZ plane if airborne)
        Vector3 velProjected = isGrounded ? Vector3.ProjectOnPlane(body.velocity, slopeHit.normal) : new(body.velocity.x, 0, body.velocity.z);

        // Apply the drag force (also compensate for jittering around when velocity is too low)
        //body.AddForce(-dragIntensity * (velProjected.sqrMagnitude >= 0.25f ? velProjected.normalized : velProjected));
        body.AddForce(-dragIntensity * velProjected);
    }

    // Helper function to clamp horizontal speed
    private void ClampHorizSpeed(RaycastHit hit, bool isSlopeWall)
    {
        Vector3 velFlat = new(body.velocity.x, 0, body.velocity.z);
        velFlat = Mathf.Clamp(velFlat.magnitude, 0, maxWalkSpeed) * velFlat.normalized;
        velFlat.y = body.velocity.y;
        body.velocity = velFlat;
        // Keep player from bouncing on a ramp when going up
        //if (isGrounded && !isSlopeWall)
        //    body.AddForce(-hit.normal * 10);
    }
}
