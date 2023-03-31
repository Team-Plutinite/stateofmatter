using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheckpointData;

public enum PlayerMoveState
{
    Idle,
    IdleCrouched,
    Moving,
    MovingCrouched,
    Jumping,
    Airborne
}

public class PlayerController : MonoBehaviour
{// Instance Editable variables
    [Header("Walk & Crouch Settings")]
    public float maxWalkSpeed = 5.0f;
    public float movementAccelGround = 50.0f;
    public float crouchSpeedMultiplier = 0.5f;
    public float movementAccelAir = 2.0f;
    public float groundDrag = 10.0f;
    [Tooltip("Set the max incline angle the player can walk up, in degrees")]
    public float maxIncline = 47.5f;

    // jumping variables
    [Header("Jump Settings")]
    public float airDrag = 2.0f;
    public float jumpHeight = 300.0f;
    public float jumpCooldown = 0.25f;

    // dashing variables
    [Header("Dash Settings")]
    public float dashCooldown = 0.5f;
    public float dashDistance = 5.0f;
    public float dashTime = 0.33f;

    [Header("Look Settings")]
    public float lookSensitivity = 2.5f;

    // audio
    [Header("Audio Settings")]
    public AudioSource source;
    public AudioClip moveSound;
    public AudioClip dashSound;
    public AudioClip jumpSound;

    // Store the player's input movement
    private Vector3 inputDir;

    // Camera variables
    private Transform camTransform;
    private Vector2 camPitchYaw = Vector2.zero;
    private Quaternion camRotQuat;
    private float zRecoilTarget = 0, zRecoilSmooth = 0;

    // Cutscene camera stuff
    [Header("Cutscene Mode")]
    public float maxAngleOffset = 20.0f;

    private Vector3 cutsceneLookDir, targetLookDir, currentLookDir;
    private readonly float slowCone = 0.5f; // 60-deg
    private readonly float stopCone = 0.9998f; // 1-deg

    // grounded, crouched, ladder, lock states
    [Header("Exposed Debug States")]
    [SerializeField]
    private bool movementLocked;
    [SerializeField]
    private bool isGrounded;
    private Vector3 groundNormal; // Surface normal of where player is stepping
    [SerializeField]
    private bool isCrouched;
    private float jumpTime;
    [SerializeField]
    private bool onLadder;

    // Internal dash data
    private float dashCoolCountdown, dashEventCountdown;
    private bool tryDash;
    private Vector3 dashDirection;

    // Reference to player rigid body
    private Rigidbody body;
    private PlayerMoveState moveState;

    public bool hasGun;

    private GameObject playerGun;
    private GameObject playerArms;

    // Start is called before the first frame update
    void Start()
    {
        moveState = PlayerMoveState.Idle;
        camTransform = transform.GetChild(0);
        body = GetComponent<Rigidbody>();

        isGrounded = false;
        isCrouched = false;
        onLadder = false;

        jumpTime = 0.0f;
        dashCoolCountdown = 0.0f;
        dashEventCountdown = dashTime;
        tryDash = false;
        dashDirection = Vector3.zero;
        groundNormal = Vector3.zero;

        cutsceneLookDir = transform.forward;
        targetLookDir = transform.forward;
        currentLookDir = transform.forward;

        Cursor.lockState = CursorLockMode.Locked; // lock to middle of screen and set invisible

        source = gameObject.AddComponent<AudioSource>();
        source.volume = 0.2f;

        playerGun = GameObject.Find("Player/CameraFollower/Gun_Problem");
        playerArms = GameObject.Find("Player/CameraFollower/SM_Player_SCR/SM_Player_Armed");
    }

    // Update is called once per frame
    void Update()
    {
        // -- CUTSCENE MODE -- \\
        if (movementLocked)  
        {
            // CAMERA CONTROL (Smooth Mode) \\

            targetLookDir = Matrix4x4.Rotate(Quaternion.Euler(new(0, Input.GetAxisRaw("Mouse X") * lookSensitivity, 0)) * 
                Quaternion.AngleAxis(-Input.GetAxisRaw("Mouse Y") * lookSensitivity, camTransform.right)) * targetLookDir;

            // Clamp view angle inside the allowed look cone
            float maxAngle = Mathf.Lerp(maxAngleOffset, 0, Mathf.Pow(Mathf.Abs(cutsceneLookDir.y), 3)); // look cone gets smaller as camera looks up or down
            if (Mathf.Acos(Vector3.Dot(cutsceneLookDir, targetLookDir)) * Mathf.Rad2Deg > maxAngle)
                targetLookDir = Matrix4x4.Rotate(Quaternion.AngleAxis(maxAngle, Vector3.Cross(cutsceneLookDir, targetLookDir).normalized)) * cutsceneLookDir;

            // Smooth the camera rotational velocity between the slow and stop angles (currently a linear interpolation)
            float rotationalVel = Vector3.Dot(targetLookDir, currentLookDir) > stopCone ? 0 
                : 
                Mathf.Min(1f - (Vector3.Dot(targetLookDir, currentLookDir) - slowCone) / (stopCone - slowCone), 1f) * lookSensitivity * 100;
            // Rotate the actual look direction the player sees by this value
            currentLookDir = Matrix4x4.Rotate(Quaternion.AngleAxis(rotationalVel * Time.deltaTime, Vector3.Cross(currentLookDir, targetLookDir).normalized)) * currentLookDir;
            
            camRotQuat = Quaternion.LookRotation(Vector3.Normalize(new(currentLookDir.x, 0, currentLookDir.z)), Vector3.up);
            body.MoveRotation(camRotQuat.normalized);
            camTransform.localRotation = Quaternion.Euler(Mathf.Asin(-currentLookDir.y) * Mathf.Rad2Deg, 0, 0);
        }

        // -- GAMEPLAY MODE -- \\ 
        else
        {
            // CAMERA CONTROL \\
            camPitchYaw.x += Input.GetAxisRaw("Mouse X") * lookSensitivity;
            camPitchYaw.y = Mathf.Clamp(camPitchYaw.y + Input.GetAxisRaw("Mouse Y") * lookSensitivity, -89f, 89f);

            camRotQuat.eulerAngles = new(0, camPitchYaw.x, 0);
            body.MoveRotation(camRotQuat.normalized); // camera pitch (also character transform pitch)
            camTransform.localRotation = Quaternion.Euler(-camPitchYaw.y, 0, 0); // camera yaw

            // Handle camera backwards recoil
            zRecoilSmooth = Mathf.Clamp(0, zRecoilSmooth - Time.deltaTime/3, 0.25f);
            camTransform.localPosition = new(camTransform.localPosition.x, camTransform.localPosition.y, -zRecoilSmooth);

            // GROUND-ONLY MOVEMENT CONTROLS \\
            if (isGrounded && jumpTime <= 0.0f)
            {
                // CROUCH/UNCROUCH \\
                if (Input.GetKey(KeyCode.LeftControl))
                    TryCrouch();
                else
                    TryUncrouch();

                // JUMP \\
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    jumpTime = jumpCooldown;
                    isGrounded = false;
                    // Reset vertical vel
                    body.velocity = new(body.velocity.x, 0, body.velocity.z);
                    body.AddForce(transform.up * jumpHeight);
                    TryUncrouch(); // uncrouch the player if they're crouching
                    source.PlayOneShot(jumpSound);
                }
            }

            // ACTIVATE DASH ABILITY \\
            if (dashCoolCountdown <= 0 && Input.GetKeyDown(KeyCode.LeftShift)) tryDash = true;

            // COOLDOWN REDUCTIONS \\
            dashCoolCountdown -= Time.deltaTime;
            jumpTime -= Time.deltaTime;

            if (hasGun && !playerGun.activeSelf)
            {
                playerGun.SetActive(true);
                //playerArms.SetActive(true);
            } else if (!hasGun && playerGun.activeSelf)
            {
                playerGun.SetActive(false);
                //playerArms.SetActive(false);
            }
            
            playerArms.SetActive(playerGun.activeSelf);
        }

        // for testing; spawn an enemy
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Physics.Raycast(transform.position, camTransform.forward, out RaycastHit hit, 100, ~(1 << 3 | 1 << 6)))
                GameObject.Find("EnemyManager").GetComponent<EnemyManager>().SpawnEnemy(100, hit.point, Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.Minus)) CutsceneMode = true;
        if (Input.GetKeyDown(KeyCode.Equals)) CutsceneMode = false;

        // kill player
        if (Input.GetKeyDown(KeyCode.K))
            gameObject.GetComponent<PlayerStats>().hp = 0;
    }

    private void FixedUpdate()
    {
        isGrounded = CheckAirborne(); // Updates isGrounded
        body.useGravity = !(isGrounded || onLadder); // so player isn't sliding down a slope
        
        Vector3 movementForce = Vector3.zero;
        // Move around; Only allowed when not in cutscene mode
        if (!movementLocked)
        {
            // -- BASIC MOVEMENT HANDLING -- \\
            if (Input.GetKey(KeyCode.W)) movementForce += transform.forward;
            if (Input.GetKey(KeyCode.S)) movementForce -= transform.forward;
            if (Input.GetKey(KeyCode.A)) movementForce -= transform.right;
            if (Input.GetKey(KeyCode.D)) movementForce += transform.right;
        }

        inputDir = Vector3.ProjectOnPlane(movementForce, groundNormal).normalized;

        // Accelerate the player in their movement direction and apply ground drag force
        body.AddForce((isGrounded ?
            isCrouched ? crouchSpeedMultiplier * movementAccelGround : movementAccelGround :
            movementAccelAir) * inputDir);

        // APPLY WALKING DRAG FORCE \\

        // Get velocity of player projected onto the surface walked on (or the XZ plane if airborne)
        Vector3 velProjected = isGrounded ? Vector3.ProjectOnPlane(body.velocity, groundNormal) :
            new(body.velocity.x, 0, body.velocity.z);
        body.AddForce(-(isGrounded ? groundDrag : airDrag) * velProjected);

        // -- DASH ABILITY -- \\

        if (tryDash)
        {
            // For the first dash frame, set the dash direction and turn off gravity (and play sound)
            if (dashCoolCountdown <= 0)
            {
                dashCoolCountdown = dashCooldown;
                dashDirection = (inputDir.sqrMagnitude > 0 ? inputDir :
                    Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized) * maxWalkSpeed;
                body.useGravity = false;
                source.PlayOneShot(dashSound);
            }
            if (dashEventCountdown > 0) // Every subsequent frame, do the dash thing
            {
                dashEventCountdown -= Time.fixedDeltaTime;
                body.velocity = dashDirection.normalized * (dashDistance / dashTime);
            }
            else // Once dash time is out, stop dashing
            {
                body.velocity = dashDirection;
                tryDash = false;
                body.useGravity = true;
                dashEventCountdown = dashTime;
            }
        }
    }

    // Returns the current movement state of the player.
    PlayerMoveState MoveState { get { return moveState; } }

    // Set the player's control state between gameplay and cutscene (true for cutscene)
    public bool CutsceneMode
    {
        get { return movementLocked; }
        set 
        {
            if (value)
            {
                cutsceneLookDir = camTransform.forward;
                currentLookDir = camTransform.forward;
                targetLookDir = camTransform.forward;
                movementLocked = true;
                return;
            }
            camPitchYaw.x = transform.eulerAngles.y;
            camPitchYaw.y = camTransform.eulerAngles.x > 90 ? 360 - camTransform.eulerAngles.x : -camTransform.eulerAngles.x;
            movementLocked = false;
        }
    }

    /// <summary>
    /// Adds an offset to the camera in the backwards direction to simulate recoil in the z-direction
    /// </summary>
    /// <param name="value">The value to add to the current recoil value</param>
    public void AddZRecoil(float value)
    {
        zRecoilSmooth += value;
    }

    // Get or Set the Cutscene Mode Looking Direction
    public Vector3 CutsceneLookDir { set { cutsceneLookDir = value; } get { return cutsceneLookDir; } }

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
            // If there's nothing above the player, uncrouch
            if (!Physics.SphereCast(new(transform.position, Vector3.up), GetComponent<CapsuleCollider>().radius, GetComponent<CapsuleCollider>().height))
            {
                isCrouched = false;
                GetComponent<CapsuleCollider>().height *= 2;
            }
        }
    }

    // Check if the player is airborne with a sphere cast down
    bool CheckAirborne()
    {
        if (Physics.SphereCast(transform.position, GetComponent<CapsuleCollider>().radius,
            Vector3.down, out RaycastHit hit, GetComponent<CapsuleCollider>().height * 0.51f - GetComponent<CapsuleCollider>().radius))
        {
            groundNormal = hit.normal;
            if (Vector3.Dot(Vector3.down, groundNormal) <= -Mathf.Cos(maxIncline * Mathf.Deg2Rad))
            {
                return true;
            }
        }
        groundNormal = transform.up;
        return false;
    }

    public Vector3 InputDirection { get { return inputDir; } }

    public bool OnLadder { get { return onLadder; } set { onLadder = value; } }
}