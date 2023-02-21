using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum Modes //Will be used later to differentiate firing modes
{
    Water = 0,
    Steam = 1,
    Ice = 2
}


public class Weapon : MonoBehaviour
{
    Modes currentMode = Modes.Steam;
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
    
    

    [Space]

    [SerializeField]
    private int dps = 3;
    
    [SerializeField]
    private float effectDur = 3f;

    // audio
    [SerializeField]
    public AudioSource source;
    private float fireSoundTimer;
    private float fireSoundCooldown;
    [SerializeField]
    public AudioClip steamFireSound;

    private void Awake()
    {
        FiringSystem = new ParticleSystem[3] { waterSystem, steamSystem, iceSystem };
        source = gameObject.AddComponent<AudioSource>();
        source.volume = 0.1f;
        fireSoundTimer = 0.0f;
        fireSoundCooldown = 0.52f;
        
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
                currentMode = Modes.Water;
                
            }
            Debug.Log(currentMode.ToString());
        }

        fireSoundTimer -= Time.deltaTime;
    }


    //Sets particle system and hitbox to turn on and off respectively 
    private void Fire()
    {
        FiringSystem[(int)currentMode].gameObject.SetActive(true);
        AttackRadius.gameObject.SetActive(true);
        source.loop = true;
        if (currentMode == Modes.Steam)
        {
            fireSoundCooldown = 0.52f; //setting cooldown to length of audio clip
            if (fireSoundTimer <= 0.0f)
            {
                source.PlayOneShot(steamFireSound); //playing audio
                fireSoundTimer = fireSoundCooldown; //reset timer
            }
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
