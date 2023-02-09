using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerMoveState
{
    Idle,
    IdleCrouched,
    Moving,
    MovingCrouched,
    Jumping,
    Airborne
}

public class PlayerController : MonoBehaviour
{
    private PlayerMoveState moveState;

    // Instance Editable variables
    public float maxWalkSpeed = 5.0f;
    public float movementAccelGround = 50.0f;
    public float crouchSpeedMultiplier = 0.5f;
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
    private bool isSlopeWall;
    private Vector3 groundNormal; // Surface normal of where player is stepping
    [SerializeField]
    private bool isCrouched;
    private float jumpTime;

    private Dictionary<GameObject, List<ContactPoint>> collisionMap;

    // Start is called before the first frame update
    void Start()
    {
        moveState = PlayerMoveState.Idle;
        camTransform = transform.GetChild(0);
        body = GetComponent<Rigidbody>();

        isGrounded = false;
        isCrouched = false;
        jumpTime = 0.0f;
        groundNormal = Vector3.zero;
        collisionMap = new();

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
        if (isGrounded && jumpTime <= 0.0f)
        {            // -- CROUCH/UNCROUCH -- \\
            if (Input.GetKey(KeyCode.LeftControl))
                TryCrouch();
            else
                TryUncrouch();

            // -- JUMP -- \\
            if (Input.GetKeyDown(KeyCode.Space))
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
        CheckAirborne();
        body.useGravity = !isGrounded; // so player isn't sliding down a slope

        // -- BASIC MOVEMENT HANDLING -- \\

        Vector3 movementForce = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movementForce += transform.forward;
        if (Input.GetKey(KeyCode.S)) movementForce -= transform.forward;
        if (Input.GetKey(KeyCode.A)) movementForce -= transform.right;
        if (Input.GetKey(KeyCode.D)) movementForce += transform.right;
        if (!isSlopeWall) movementForce = Vector3.ProjectOnPlane(movementForce, groundNormal).normalized;

        // Accelerate the player in their movement direction and apply ground drag force
        body.AddForce((isGrounded ? 
            isCrouched ? crouchSpeedMultiplier * movementAccelGround : movementAccelGround : 
            movementAccelAir) * movementForce);

            // APPLY WALKING DRAG FORCE \\

        // Get velocity of player projected onto the surface walked on (or the XZ plane if airborne)
        Vector3 velProjected = isGrounded ? Vector3.ProjectOnPlane(body.velocity, groundNormal) : 
            new(body.velocity.x, 0, body.velocity.z);
        body.AddForce(-(isGrounded ? groundDrag : airDrag) * velProjected);

            // CLAMP WALK SPEED \\

        Vector3 velFlat = new(body.velocity.x, 0, body.velocity.z);
        velFlat = Mathf.Clamp(velFlat.magnitude, 0, maxWalkSpeed) * velFlat.normalized;
        body.velocity = new(velFlat.x, body.velocity.y, velFlat.z);
    }

    // Returns the current movement state of the player.
    PlayerMoveState MoveState { get; }

    // Attempt a crouch
    void TryCrouch()
    {
        if (!isCrouched)
        {
            isCrouched = true;
            GetComponent<CapsuleCollider>().height /= 2;
            Debug.Log("asd");
            body.AddForce(new(0, -250)); // push the player down so they aren't floating for a second
        }
    }

    // Uncrouch, if currently crouched
    void TryUncrouch()
    {
        if (isCrouched)
        {
            // If there's nothing above the player, uncrouch
            if (!Physics.SphereCast(new(transform.position, Vector3.up), GetComponent<CapsuleCollider>().radius, GetComponent<CapsuleCollider>().height))
            {
                isCrouched = false;
                GetComponent<CapsuleCollider>().height *= 2;
            }
        }
    }

    // Check all contact points to see if any of them are ground points
    // If there are ground points, the player is grounded.
    void CheckAirborne()
    {
        Queue<ContactPoint> totalContacts = new();
        bool possibleGround = false;
        bool possibleSlopeWall = false;
        Vector3 avgNormal = Vector3.zero;

        // Add all contact points to the list
        foreach (GameObject key in collisionMap.Keys)
        {
            foreach (ContactPoint pt in collisionMap[key])
                totalContacts.Enqueue(pt);
        }

        if (totalContacts.Count > 0)
        {
            ContactPoint c;
            while (totalContacts.TryDequeue(out c))
            {
                if (transform.position.y - (c.point.y + c.separation) > GetComponent<CapsuleCollider>().height * 0.3f)
                {
                    possibleGround = true;
                    avgNormal += c.normal;
                }
            }
            avgNormal.Normalize();

            if (possibleGround && Vector3.Dot(Vector3.down, avgNormal) > -Mathf.Cos(maxIncline * Mathf.Deg2Rad))
            {
                possibleSlopeWall = true;
                possibleGround = false;
            }
        }
        groundNormal = avgNormal;
        isGrounded = possibleGround;
        isSlopeWall = possibleSlopeWall;
    }

    private void OnCollisionStay(Collision collision)
    {
        List<ContactPoint> pts = new();
        int ptCount = collision.GetContacts(pts);

        collisionMap[collision.gameObject] = pts.GetRange(0, ptCount);
    }
    private void OnCollisionExit(Collision collision)
    {
        collisionMap.Remove(collision.gameObject);
    }
}
