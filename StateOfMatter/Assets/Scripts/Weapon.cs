using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum Modes //Will be used later to differentiate firing modes
{
    Water,
    Steam,
    Ice
}


public class Weapon : MonoBehaviour
{
    //Handles the particles that come out when firing.
    [SerializeField]
    private ParticleSystem FiringSystem;
    [SerializeField]
    private WeaponAttackRadius AttackRadius;

    [Space]

    [SerializeField]
    private int dps = 3;
    private float effectDur = 3f;

    private void Awake()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Firing");
            Fire();
        }
        else
        {
            StopFiring();
        }
    }


    //Sets particle system and hitbox to turn on and off respectively 
    private void Fire()
    {
        FiringSystem.gameObject.SetActive(true);
        AttackRadius.gameObject.SetActive(true);
    }

    private void StopFiring()
    {
        FiringSystem.gameObject.SetActive(false);
        AttackRadius.gameObject.SetActive(false);
    }
}
