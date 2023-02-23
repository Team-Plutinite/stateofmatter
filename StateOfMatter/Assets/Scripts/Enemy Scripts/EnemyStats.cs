using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public enum MatterState
{
    Ice = 0,
    Water = 1,
    Gas = 2,
    None = 3
}

public class EnemyStats : MonoBehaviour
{
    public EnemyManager manager;

    private float maxHP;
    private MatterState debuffState;
    private float debuffTime;
    private float hp;
    private float dotTime;
    private float dotDmg;
    private float heatAmt, iceAmt;
    private float debuffMax;

    private float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
    }

    /// <summary>
    /// Initialize this enemy's values. Ideally this is only 
    /// called from EnemyManager for spawning purposes
    /// </summary>
    /// <param name="hp">The HP of the spawned enemy</param>
    public void Init(float hp, Vector3 position, Vector3 pitchYawRoll)
    {
        // Set transform data
        transform.SetPositionAndRotation(position, Quaternion.Euler(pitchYawRoll));

        debuffState = MatterState.None;
        debuffTime = 0.0f;
        this.hp = hp;
        maxHP = hp;
        dotTime = 0.0f;
        dotDmg = 0.0f;
        gameObject.SetActive(true);
        heatAmt = iceAmt = 0;
        debuffMax = 1.5f;

        moveSpeed = GetComponent<NavMeshAgent>().speed;
    }

    // Update is called once per frame
    void Update()
    {
        dotTime -= Time.deltaTime;
        if (dotTime > 0f)
            TakeDamage(dotDmg * Time.deltaTime);

        debuffTime -= Time.deltaTime;
        // If debuff timer runs out, neutralize debuffs
        if (debuffTime <= 0.0f && debuffState != MatterState.None)
            NeutralizeDebuffs();

        // Decrease freeze and heat amounts
        if (heatAmt > 0) heatAmt -= Time.deltaTime / 2;
        if (iceAmt > 0) iceAmt -= Time.deltaTime / 2;
        GetComponent<NavMeshAgent>().speed = moveSpeed * (1-(iceAmt / debuffMax));

        Material mat = GetComponent<Renderer>().material;
        // Visually show debuff state on enemy
        switch (debuffState)
        {
            case MatterState.Ice:
                mat.SetColor("_Color", Color.cyan);
                break;
            case MatterState.Water:
                mat.SetColor("_Color", Color.blue + new Color(heatAmt / debuffMax, iceAmt / debuffMax, iceAmt / debuffMax, 1));
                break;
            case MatterState.Gas:
                mat.SetColor("_Color", Color.red);
                break;
            default:
                mat.SetColor("_Color", Color.white);
                break;
        }
    }

    /// <summary>
    /// Afflict this enemy with a specific Matter debuff: Ice, Water, or Gas
    /// </summary>
    /// <param name="state">The Matter State to apply</param>
    /// <param name="seconds">How long this debuff should last</param>
    public void Afflict(MatterState state, float seconds)
    {
        if (debuffState == MatterState.Water && state != MatterState.None)
            debuffTime = seconds;

        switch (state)
        {
            case MatterState.Ice:
                // Freeze the enemy if they are currently wet
                if (debuffState == MatterState.Water)
                {
                    iceAmt += Time.deltaTime * 2;
                    if (iceAmt >= debuffMax)
                    {
                        Freeze();
                        iceAmt = 0;
                    }
                }  
                break;

            case MatterState.Water:
                // Debuff the enemy with Wet, but only if they are not already debuffed
                if (debuffState == MatterState.None)
                {
                    debuffState = MatterState.Water;
                    debuffTime = seconds;
                }
                break;

            case MatterState.Gas:
                // Burst the enemy if they are wet
                if (debuffState == MatterState.Water)
                {
                    heatAmt += Time.deltaTime * 2;
                    if (heatAmt >= debuffMax)
                    {
                        Burst(seconds);
                        heatAmt = 0;
                    }
                }
                break;

            default: break; // state.None does nothing
        }
    }

    // Freezes the enemy, leaving them unresponsive
    public void Freeze()
    {
        debuffState = MatterState.Ice;
        GetComponent<NavMeshAgent>().isStopped = true;
        // Nothing yet
    }

    // Burst the enemy, doing single-shot AOE then adding individual DOT effect
    public void Burst(float seconds)
    {
        debuffState = MatterState.Gas;
        

        manager.CreateAOE(transform.position, 4.0f, a =>
        {
            a.GetComponent<Rigidbody>().AddExplosionForce(3000f, transform.position, 4f);
            a.GetComponent<EnemyStats>().TakeDamage(35.0f);
        });

        ApplyDOT(50, seconds);
    }

    // Eliminate enemy debuffs
    public void NeutralizeDebuffs()
    {
        debuffState = MatterState.None;
        GetComponent<NavMeshAgent>().isStopped = false;
    }

    public void TakeDamage(float dmgAmt)
    {
        hp -= dmgAmt;
        transform.GetComponentInChildren<TextMeshPro>().text = hp.ToString();

        if (hp <= 0) manager.KillEnemy(gameObject.GetInstanceID());
    }

    /// <summary>
    ///  Apply a Damage-Over-Time effect to this enemy.
    /// </summary>
    /// <param name="seconds">Amount of time this effect lasts, in seconds.</param>
    /// <param name="totalDamage">The total damage to deal over this time.</param>
    public void ApplyDOT(float totalDamage, float seconds)
    {
        dotTime = seconds;
        dotDmg = totalDamage / seconds;
    }
}

