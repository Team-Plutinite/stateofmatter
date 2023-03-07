using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Weapon : MonoBehaviour
{
    // GameObject references
    private EnemyManager enemyManager;
    private GameObject player;

    MatterState currentMode = MatterState.Ice;
    //Handles the particles that come out when firing.
    [SerializeField]
    private ParticleSystem gasSystem;

    [SerializeField]
    private ParticleSystem liquidSystem;

    [SerializeField]
    private ParticleSystem solidSystem;

    private ParticleSystem[] FiringSystem;

    [SerializeField]
    private WeaponAttackRadius AttackRadius;

    [Space]

    [Header("Pulse Ability")]
    [Tooltip("The angle IN DEGREES of the pulse cone (angle relative to forward axis).")]
    public float pulseAngle = 45.0f;
    [Tooltip("The range of the pulse.")]
    public float pulseRange = 3.0f;
    [Tooltip("The force of the pulse to apply to enemies caught in it.")]
    public float pulseForce = 150.0f;

    [Space]

    [Header("Solid Shotgun")]
    [Tooltip("The angle IN DEGREES of the shotgun pellet spread (angle relative to forward axis)")]
    public float spreadAngle = 20.0f;
    [Tooltip("The maximum range of the shotgun. Enemies cannot be hit from farther than this distance.")]
    public float maxRange = 30.0f;
    [Tooltip("The number of pellets shot with each blast.")]
    public int pelletCount = 6;
    [Tooltip("The damage to do per pellet.")]
    public float pelletDamage = 5.0f;
    
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
        player = GameObject.FindGameObjectWithTag("Player");
        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        FiringSystem = new ParticleSystem[3] { solidSystem, liquidSystem, gasSystem  };
        AttackRadius.OnStay += DamageEnemy;

        AttackRadius.MeltEnter += DamageMeltable;

        //FiringSystem = new ParticleSystem[3] { waterSystem, steamSystem, iceSystem };
        source = gameObject.AddComponent<AudioSource>();
        source.volume = 0.2f;
        fireSoundTimer = 0.0f;
        fireSoundCooldown = 0.52f;
        
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
        // Primary fire - differs depending on state
        TryFire();

        // Pulse ability - apply a knockback to all enemies in front in a cone
        if (Input.GetMouseButtonDown(1))
        {
            foreach (GameObject enemy in enemyManager.Enemies.Values)
            {
                Vector3 enemyDir = enemy.transform.position - player.transform.position;

                // If enemy is in pulse range
                if (enemyDir.sqrMagnitude < Mathf.Pow(pulseRange, 2) && 
                    Mathf.Acos(Vector3.Dot(player.transform.forward, enemyDir.normalized)) * Mathf.Rad2Deg < pulseAngle)
                    enemy.GetComponent<EnemyStats>().Knockback(player.transform.position, pulseForce);
            }
        }

        //Press the r key to cycle through MatterState
        if (Input.GetKeyDown(KeyCode.R))
        {
            FiringSystem[(int)currentMode].gameObject.SetActive(false);

            currentMode++;

            if((int)currentMode > 2)
                currentMode = MatterState.Ice;

            FiringSystem[(int)currentMode].gameObject.SetActive(true);
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

    //Call Afflict when the enemy gets into the attack radius
    private void DamageEnemy(EnemyStats enemy)
    {
        enemy.Afflict(GetMatterState(), effectDur);
    }

    //Begin Damaging ice when it enters the radius.
    private void DamageMeltable(Meltable melt)
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
        // activate particles
        FiringSystem[(int)currentMode].gameObject.SetActive(true);

        //AttackRadius.gameObject.SetActive(true);
        source.loop = true;

        // Determine the firing mode
        switch (currentMode)
        {
            case MatterState.Ice: //ice
                if (Input.GetMouseButtonDown(0))
                {
                    // Shotgun blast
                    ShotgunAttack(spreadAngle, maxRange, pelletCount, pelletDamage);

                    fireSoundCooldown = 0.83f;
                    if (fireSoundTimer <= 0.0f)
                    {
                        source.PlayOneShot(freezeFireSound);
                        fireSoundTimer = fireSoundCooldown;
                    }
                }

                break;

            case MatterState.Water:
                if (Input.GetMouseButton(0))
                {
                    // TODO: implement laser
                }

                break;

            case MatterState.Gas: //gas

                if (Input.GetMouseButton(0))
                {
                    // TODO: implement cloud thingy

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
    /// Shoot the weapon with a specifed spread angle, range, pellet count, and pellet damage
    /// </summary>
    /// <param name="spreadAngle">The cone angle of the shotgun blast, in degrees.</param>
    /// <param name="maxRange">The maximum range pellets can reach.</param>
    /// <param name="numPellets">The number of pellets to shoot. These pellets will be uniform around the cone.</param>
    /// <param name="dmgPerPellet">How much damage an enemy takes from a single pellet.</param>
    private void ShotgunAttack(float spreadAngle, float maxRange, int numPellets, float dmgPerPellet)
    {
        // Clamp spread angle and convert to rads
        spreadAngle = Mathf.Clamp(spreadAngle, 0, 90.0f) * Mathf.Deg2Rad;

        List<GameObject> enemies = new();
        GameObject[] enemyArr = new GameObject[enemyManager.Enemies.Count];
        enemyManager.Enemies.Values.CopyTo(enemyArr, 0);
        enemies.AddRange(enemyArr);
        
        // First, cull out any enemies not in the cone; they are guarenteed to not get hit
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            Vector3 enemyDir = (enemies[i].transform.position - player.transform.position).normalized;

            if (Vector3.Dot(player.transform.forward, enemyDir) < Mathf.Cos(spreadAngle))
                enemies.RemoveAt(i);
        }

        // Next, raycast out in every pellet direction and hit enemies.
        for (float theta = 0.0f; theta < 2.0f * Mathf.PI; theta += (Mathf.PI * 2.0f) / numPellets)
        {
            // The length of the Z component of the forward direction is based on how high the cone's spread angle is.
            // X and Y components combined are unit length and rotate around based on theta.
            // Then simply normalize this whole thing to get the resulting direction.
            Vector3 pelletDir = Vector3.Normalize(new(Mathf.Cos(theta), Mathf.Sin(theta), 1 / Mathf.Tan(spreadAngle)));

            // This vec is in local space; transform it to world space relative to player camera.
            pelletDir = player.transform.GetChild(0).localToWorldMatrix.MultiplyVector(pelletDir);

            // Unity bitmask is 32 bits; Player's layer mask is the 6th bit.
            // Since we want to raycast against everything BUT that, invert the bitmask.
            int layerMask = ~(1 << 6); // 1111 1111 1111 1111 1111 1111 1101 1111

            // Did we hit an enemy?
            if (Physics.Raycast(player.transform.position, pelletDir, out RaycastHit hit, maxRange, layerMask))
            {
                if (enemies.Contains(hit.transform.gameObject))
                {
                    EnemyStats statsComponent = hit.transform.gameObject.GetComponent<EnemyStats>();
                    statsComponent.TakeDamage(dmgPerPellet);
                    statsComponent.Afflict(currentMode, 5.0f);
                }
            }
        }

        
    }
}
