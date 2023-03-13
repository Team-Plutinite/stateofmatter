using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Weapon : MonoBehaviour
{
    // GameObject references
    private EnemyManager enemyManager;
    private GameObject player;
    private GameObject playerCam;

    private MatterState currentMode = MatterState.Ice;
    //Handles the particles that come out when firing.
    [SerializeField]
    private ParticleSystem gasSystem;
    [SerializeField]
    private ParticleSystem liquidSystem;
    [SerializeField]
    private ParticleSystem solidSystem;
    private ParticleSystem[] FiringSystem;

    [Space]

    [Header("Pulse Ability")]
    [Tooltip("The angle IN DEGREES of the pulse cone (angle relative to forward axis).")]
    public float pulseAngle = 45.0f;
    [Tooltip("The range of the pulse.")]
    public float pulseRange = 3.0f;
    [Tooltip("The force of the pulse to apply to enemies caught in it.")]
    public float pulseForce = 150.0f;
    [Tooltip("Pulse ability cooldown, in seconds.")]
    public float pulseCooldown = 1.0f;
    private float pulseTimer;

    [Space]

    [Header("Solid Shotgun")]
    [Tooltip("The inner angle IN DEGREES of the shotgun pellet spread (angle relative to forward axis)")]
    public float innerSpreadAngle = 7.5f;
    [Tooltip("The outer angle IN DEGREES of the shotgun pellet spread (angle relative to forward axis)")]
    public float outerSpreadAngle = 20.0f;
    [Tooltip("The maximum range of the shotgun. Enemies cannot be hit from farther than this distance.")]
    public float solidRange = 30.0f;
    [Tooltip("The number of inner pellets shot with each blast.")]
    public int innerPelletCount = 3;
    [Tooltip("The number of outer pellets shot with each blast.")]
    public int outerPelletCount = 7;
    [Tooltip("The damage to do per pellet.")]
    public float pelletDamage = 5.0f;
    [Tooltip("Solid Mode Rounds Per Minute.")]
    public float solidRPM = 75.0f;
    private float solidAtkTimer;

    [Space]

    [Header("Liquid Laser")]
    [Tooltip("Damage per laser shot.")]
    public float liquidDmg = 1.7f;
    [Tooltip("Max range of the laser.")]
    public float liquidRange = int.MaxValue;
    [Tooltip("Liquid Mode Rounds Per Minute.")]
    public float liquidRPM = 1000.0f;
    private float liquidAtkTimer;

    [Space]

    [Header("Gas Cloud Emitter")]
    [Tooltip("Gas cloud's damage per second.")]
    public float gasDmg = 5.0f;
    [Tooltip("The total lifetime of gas clouds.")]
    public float gasLife = 5.0f;
    [Tooltip("Max range of the gas cloud.")]
    public float gasRange = int.MaxValue;
    [Tooltip("Gas Mode Rounds (clouds) Per Minute.")]
    public float gasRPM = 180.0f;
    private float gasAtkTimer;
    [Tooltip("Gas cloud spawn pool count.")]
    public int gasCloudPoolCount = 50;
    private Queue<GameObject> gasClouds;
    public GameObject gasCloudPrefab;

    [Space]

    [Tooltip("Show a line renderer that represents the spread of pellets. Each " +
        "vertex in the circle represents the point through which a ray is being cast.")]
    public bool debug;
    private LineRenderer debugLines;

    [Space]

    [SerializeField]
    private float effectDur = 5f;

    // audio
    [SerializeField]
    public AudioSource source;
    private float fireSoundTimer;
    private float fireSoundCooldown;
    [SerializeField]
    public AudioClip steamFireSound;
    [SerializeField]
    public AudioClip freezeFireSound;

    private void Awake()
    {
        solidAtkTimer = liquidAtkTimer = gasAtkTimer = pulseTimer = 0.0f;

        player = GameObject.FindGameObjectWithTag("Player");
        playerCam = player.transform.GetComponentInChildren<Camera>().gameObject;
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        FiringSystem = new ParticleSystem[3] { solidSystem, liquidSystem, gasSystem  };

        gasClouds = new Queue<GameObject>();
        // Fill the cloud pool
        for (int i = 0; i < gasCloudPoolCount; i++)
        {
            GameObject newCloud = Instantiate(gasCloudPrefab);
            newCloud.SetActive(false);
            newCloud.GetComponent<GasAttacker>().OnStay += CloudDamageEnemy;
            newCloud.GetComponent<GasAttacker>().MeltEnter += CloudDamageMeltable;
            gasClouds.Enqueue(newCloud);
        }

        //FiringSystem = new ParticleSystem[3] { waterSystem, steamSystem, iceSystem };
        source = gameObject.AddComponent<AudioSource>();
        source.volume = 0.2f;
        fireSoundTimer = 0.0f;
        fireSoundCooldown = 0.52f;

        if (debug)
        {
            debugLines = gameObject.AddComponent<LineRenderer>();
            debugLines.startWidth = 0.01f;
            debugLines.endWidth = 0.01f;
            debugLines.loop = true;
        }
    }

    public MatterState GetMatterState()
    {
        return currentMode;
    }

    public void SetMode(MatterState t)
    {
        currentMode = t;
    }

    private void Update()
    {
        // Reduce timers
        solidAtkTimer -= Time.deltaTime;
        liquidAtkTimer -= Time.deltaTime;
        gasAtkTimer -= Time.deltaTime;
        pulseTimer -= Time.deltaTime;

        // Primary fire - differs depending on state
        TryFire();

        // Pulse ability - apply a knockback to all enemies in front in a cone
        if (Input.GetMouseButtonDown(1) && pulseTimer <= 0.0f)
        {
            pulseTimer = pulseCooldown;
            GameObject[] enemyArr = new GameObject[enemyManager.Enemies.Count];
            enemyManager.Enemies.Values.CopyTo(enemyArr, 0);

            for (int i = enemyArr.Length - 1; i >= 0; i--)
            {
                Vector3 enemyDir = enemyArr[i].transform.position - player.transform.position;

                // If enemy is in pulse range
                if (enemyDir.sqrMagnitude < Mathf.Pow(pulseRange, 2) && 
                    Mathf.Acos(Vector3.Dot(player.transform.forward, enemyDir.normalized)) * Mathf.Rad2Deg < pulseAngle)
                    enemyArr[i].GetComponent<EnemyStats>().Knockback(player.transform.position, pulseForce);
            }
        }

        //Press the r key to cycle through MatterState
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentMode++;
            if((int)currentMode > 2)
                currentMode = MatterState.Ice;
        }

        //Use the number keys to switch weapons.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StopFiring(); //Resets hitbox and particles
            currentMode = MatterState.Ice;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StopFiring();
            currentMode = MatterState.Water;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StopFiring();
            currentMode = MatterState.Gas;
        }

        fireSoundTimer -= Time.deltaTime;
    }

    private void CloudDamageEnemy(EnemyStats enemy)
    {
        // If enemy already has a DOT on it, just reset its DOT timer.
        if (!enemy.ResetDebuff("RAW_GAS_DEBUFF"))
            enemy.ApplyDebuff("RAW_GAS_DEBUFF", new Debuff(0.25f, null, () => enemy.TakeDamage(Time.deltaTime * gasDmg), null));
        
        enemy.Afflict(MatterState.Gas, effectDur, Time.deltaTime);
    }

    //Begin Damaging ice when it enters the gas cloud.
    private void CloudDamageMeltable(Meltable melt)
    {
        //If the cube is fully melted, do nothing.
        if (melt.transform.localScale.x <= 0f || melt.transform.localScale.y <= 0f || melt.transform.localScale.z <= 0f)
        {
            //melt.GetWaterState().SetActive(true);]

            //Hide ice when it melts

            melt.GetMelter().GetComponent<MeshRenderer>().enabled = false;

            if(GetMatterState() == MatterState.Ice)
            {
                melt.GetMelter().GetComponent<MeshRenderer>().enabled = true;
                melt.Melt(GetMatterState());
            }
            else
            {
                return;
            }
        }
        else 
        { 
            melt.Melt(GetMatterState());
        }
    }

    //Sets particle system and hitbox to turn on and off respectively 
    private void TryFire()
    {
        source.loop = true;

        // Determine the firing mode
        switch (currentMode)
        {
            case MatterState.Ice: // Solid
                if (Input.GetMouseButtonDown(0) && solidAtkTimer <= 0.0f)
                {
                    solidAtkTimer = 1 / (solidRPM / 60.0f);
                    // Shotgun blast
                    ShotgunAttack(innerSpreadAngle, outerSpreadAngle, solidRange, innerPelletCount, outerPelletCount, pelletDamage);

                    fireSoundCooldown = 0.83f;
                    if (fireSoundTimer <= 0.0f)
                    {
                        source.PlayOneShot(freezeFireSound);
                        fireSoundTimer = fireSoundCooldown;
                    }
                }
                break;

            case MatterState.Water: // Liquid
                if (Input.GetMouseButton(0) && liquidAtkTimer <= 0.0f)
                {
                    liquidAtkTimer = 1 / (liquidRPM / 60.0f);
                    // Laser shot
                    LineAttack(liquidDmg, liquidRange);
                    // activate particles
                    FiringSystem[(int)currentMode].gameObject.SetActive(true);
                }
                break;

            case MatterState.Gas: // Gas

                if (Input.GetMouseButton(0) && gasAtkTimer <= 0.0f)
                {
                    gasAtkTimer = 1 / (gasRPM / 60.0f);

                    // Emit a gas cloud from the pool (dequeuing it, activating it, then requeuing it)
                    if (!gasClouds.Peek().activeSelf)
                    {
                        GameObject activatedCloud = gasClouds.Dequeue();
                        activatedCloud.GetComponent<GasAttacker>().Spawn(transform.position + transform.forward, transform.rotation, gasLife);
                        gasClouds.Enqueue(activatedCloud);
                    }

                    // activate particles
                    FiringSystem[(int)currentMode].gameObject.SetActive(true);

                    fireSoundCooldown = 0.52f; //setting cooldown to length of audio clip
                    if (fireSoundTimer <= 0.0f)
                    {
                        source.PlayOneShot(steamFireSound); //playing audio
                        fireSoundTimer = fireSoundCooldown; //reset timer
                    }
                }
                break;
        }

        // Player stopped shooting.
        if (Input.GetMouseButtonUp(0))
            StopFiring();
    }

    private void StopFiring()
    {
        FiringSystem[(int)currentMode].gameObject.SetActive(false);
        //AttackRadius.gameObject.SetActive(false);
        source.loop = false;
        source.Stop();
    }

    /// <summary>
    /// Shoot a single hitscan attack through middle of screen
    /// </summary>
    /// <param name="dmg">Damage of the shot</param>
    /// <param name="maxRange">The max range of the attack (if not set, will be max int)</param>
    private void LineAttack(float dmg, float maxRange = int.MaxValue)
    {
        List<GameObject> enemies = new();
        GameObject[] enemyArr = new GameObject[enemyManager.Enemies.Count];
        enemyManager.Enemies.Values.CopyTo(enemyArr, 0);
        enemies.AddRange(enemyArr);
        // Raycast through forward axis and check if it hit an enemy
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, maxRange, ~(1 << 3 | 1 << 6)))
        {
            if (enemies.Contains(hit.transform.gameObject))
            {
                EnemyStats statsComponent = hit.transform.gameObject.GetComponent<EnemyStats>();
                statsComponent.TakeDamage(dmg);
                statsComponent.Afflict(currentMode, effectDur);
            }
        }
    }

    /// <summary>
    /// Shoot the weapon with a specifed spread angle, range, pellet count, and pellet damage
    /// </summary>
    /// <param name="innerSpreadAngle">The inner cone angle of the shotgun blast, in degrees.</param>
    /// <param name="outerSpreadAngle">The outer cone angle of the shotgun blast, in degrees.</param>
    /// <param name="maxRange">The maximum range pellets can reach.</param>
    /// <param name="numInnerPellets">The number of inner pellets to shoot. These pellets will be uniform around the cone.</param>
    /// <param name="numOuterPellets">The number of outer pellets pellets to shoot. These pellets will be uniform around the cone.</param>
    /// <param name="dmgPerPellet">How much damage an enemy takes from a single pellet.</param>
    private void ShotgunAttack(float innerSpreadAngle, float outerSpreadAngle, float maxRange, int numInnerPellets, int numOuterPellets, float dmgPerPellet)
    {
        List<GameObject> enemies = new();
        List<Vector3> debugVecs = new();
        GameObject[] enemyArr = new GameObject[enemyManager.Enemies.Count];
        enemyManager.Enemies.Values.CopyTo(enemyArr, 0);
        enemies.AddRange(enemyArr);

        // Helper method to raycast the pellet trajectories and hit enemies (pellets are hitscan)
        List<Vector3> ShootCircle(float angle, float numPellets, float dmgPerPellet, float maxRange)
        {
            // clamp angle and convert to rads
            angle = Mathf.Clamp(angle, 0, 90.0f) * Mathf.Deg2Rad;
            List<Vector3> debugVecs = new();

            // Raycast out in every pellet direction and hit enemies.
            float thetaOffset = Random.value * Mathf.PI * 2;
            for (float theta = 0.0f; theta < 2.0f * Mathf.PI; theta += (Mathf.PI * 2.0f) / numPellets)
            {
                // The length of the Z component of the forward direction is based on how high the cone's spread angle is.
                // X and Y components combined are unit length and rotate around based on theta.
                // Then simply normalize this whole thing to get the resulting direction.
                Vector3 pelletDir = Vector3.Normalize(new(Mathf.Cos(theta + thetaOffset), Mathf.Sin(theta + thetaOffset), 1 / Mathf.Tan(angle)));

                // This vec is in local space; transform it to world space relative to player camera.
                pelletDir = playerCam.transform.localToWorldMatrix.MultiplyVector(pelletDir);

                // Add vector to debug renderer
                if (debug) debugVecs.Add(playerCam.transform.position + pelletDir);

                // Unity bitmask is 32 bits; Player's layer mask is the 6th bit and gas clouds are 3rd bit.
                // Since we want to raycast against everything BUT that, invert the bitmask.
                int layerMask = ~(1 << 3 | 1 << 6); // 1111 1111 1111 1111 1111 1111 1101 1011

                // Did we hit an enemy?
                if (Physics.Raycast(playerCam.transform.position, pelletDir, out RaycastHit hit, maxRange, layerMask))
                {
                    if (enemies.Contains(hit.transform.gameObject))
                    {
                        EnemyStats statsComponent = hit.transform.gameObject.GetComponent<EnemyStats>();
                        statsComponent.TakeDamage(dmgPerPellet);
                        statsComponent.Afflict(currentMode, effectDur, 0.15f);
                    }
                }
            }
            return debugVecs; // for debugging purposes
        }

        // First, cull out any enemies not in the cone; they are guarenteed to not get hit
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            Vector3 enemyDir = (enemies[i].transform.position - player.transform.position).normalized;
            if (Vector3.Dot(player.transform.forward, enemyDir) < Mathf.Cos(outerSpreadAngle))
                enemies.RemoveAt(i);
        }

        // Next, raycast out in every pellet direction and hit enemies.
        debugVecs.AddRange(ShootCircle(innerSpreadAngle, numInnerPellets, dmgPerPellet, maxRange));
        // Do it again for outer pellets
        debugVecs.AddRange(ShootCircle(outerSpreadAngle, numOuterPellets, dmgPerPellet, maxRange));
        // If debugging enabled, display the raycasts visually
        if (debug)
        {
            debugLines.positionCount = debugVecs.Count;
            debugLines.SetPositions(debugVecs.ToArray());
        }
    }
}
