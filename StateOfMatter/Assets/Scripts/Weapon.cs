using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum Modes //Will be used later to differentiate firing modes
{
    
    Ice = 0,
    Water = 1,
    Steam = 2,
    None = 3
}


public class Weapon : MonoBehaviour
{
    Modes currentMode = Modes.Ice;
    //Handles the particles that come out when firing.
    [SerializeField]
    private ParticleSystem steamSystem;

    [SerializeField]
    private ParticleSystem waterSystem;

    [SerializeField]
    private ParticleSystem iceSystem;

    private ParticleSystem[] FiringSystem;

    [SerializeField]
    private WeaponAttackRadius AttackRadius;

    private float debuffTimer;
    

    [Space]

    [SerializeField]
    private int dps = 3;
    
    [SerializeField]
    private float effectDur = 3f;

    private void Awake()
    {
        FiringSystem = new ParticleSystem[3] { iceSystem, waterSystem, steamSystem  };
        AttackRadius.OnEnter += DamageEnemy;
        AttackRadius.OnExit += StopDamage;
        debuffTimer = 0f;
    }

    public Modes GetMode()
    {
        return currentMode;
    }

    public void SetMode(Modes t)
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

        //Press the r key to cycle through modes
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentMode++;

            if((int)currentMode > 2)
            {
                currentMode = Modes.Ice;
                
            }
            Debug.Log(currentMode.ToString());
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentMode = Modes.Ice;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentMode = Modes.Water;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentMode = Modes.Steam;
        }
    }

    private void DamageEnemy(EnemyStats enemy)
    {
        enemy.Afflict((MatterState)(GetMode()));
    }

    private void StopDamage(EnemyStats enemy)
    {
        enemy.NeutralizeDebuffs();
    }

    //Sets particle system and hitbox to turn on and off respectively 
    private void Fire()
    {
        FiringSystem[(int)currentMode].gameObject.SetActive(true);
        AttackRadius.gameObject.SetActive(true);
    }

    private void StopFiring()
    {
        FiringSystem[(int)currentMode].gameObject.SetActive(false);
        AttackRadius.gameObject.SetActive(false);
    }
}
