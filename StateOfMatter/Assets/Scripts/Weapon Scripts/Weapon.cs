using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Weapon : MonoBehaviour
{
    // GameObject references
    private GameObject enemyManagerRef;
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

    [SerializeField]
    private int dps = 3;
    
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
        enemyManagerRef = GameObject.FindGameObjectWithTag("EnemyManager");
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
        if (Input.GetMouseButton(0))
        {
            Fire();
        }
        else
        {
            StopFiring();
        }

        // Pulse ability - apply a knockback to all enemies in front in a 30-degree cone
        if (Input.GetMouseButtonDown(1))
        {
            foreach (GameObject enemy in enemyManagerRef.GetComponent<EnemyManager>().Enemies.Values)
            {
                Vector3 enemyDir = enemy.transform.position - player.transform.position;

                // If enemy is in 10-unit range and angle offset from forward vector is less than 45 degrees
                if (enemyDir.sqrMagnitude < Mathf.Pow(10.0f, 2) && 
                    Mathf.Acos(Vector3.Dot(player.transform.forward, enemyDir.normalized)) * Mathf.Rad2Deg < 45.0f)
                    enemy.GetComponent<EnemyStats>().Knockback(player.transform.position, 100.0f);
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
    private void Fire()
    {
        // activate particles
        FiringSystem[(int)currentMode].gameObject.SetActive(true);

        //AttackRadius.gameObject.SetActive(true);
        source.loop = true;

        // Determine the firing mode
        switch (currentMode)
        {
            case MatterState.Ice: //ice

                fireSoundCooldown = 0.83f;
                if (fireSoundTimer <= 0.0f)
                {
                    source.PlayOneShot(freezeFireSound);
                    fireSoundTimer = fireSoundCooldown;
                }

                break;
            case MatterState.Water:
                break;
            case MatterState.Gas: //gas

                fireSoundCooldown = 0.52f; //setting cooldown to length of audio clip
                if (fireSoundTimer <= 0.0f)
                {
                    source.PlayOneShot(steamFireSound); //playing audio
                    fireSoundTimer = fireSoundCooldown; //reset timer
                }

                break;
        }
    }

    private void StopFiring()
    {
        FiringSystem[(int)currentMode].gameObject.SetActive(false);
        AttackRadius.gameObject.SetActive(false);
        source.loop = false;
        source.Stop();
    }
}
