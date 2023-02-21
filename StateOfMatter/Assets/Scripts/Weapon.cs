using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Weapon : MonoBehaviour
{
    MatterState currentMode = MatterState.Ice;
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
        AttackRadius.OnStay += DamageEnemy;
        AttackRadius.OnExit += StopDamage;
        debuffTimer = 0f;
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

        //Press the r key to cycle through MatterState
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentMode++;

            if((int)currentMode > 2)
            {
                currentMode = MatterState.Ice;
                
            }
            Debug.Log(currentMode.ToString());
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
    }

    //Call Afflict when the enemy gets into the attack radius
    private void DamageEnemy(EnemyStats enemy)
    {
        enemy.Afflict(GetMatterState(), 5f);
    }

    //Stop Damage After Leaving Radius
    private void StopDamage(EnemyStats enemy)
    {
        //enemy.NeutralizeDebuffs();
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
