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

    private float waterMoveSpeedMult;

    private float maxHP;
    private MatterState debuffState;
    private float debuffTime;
    private float hp;
    private float dotTime;
    private float dotDmg;

    private float heatAmt, iceAmt;
    private float debuffMax;

    private float stunTime;
    private float moveSpeed;

    // component references
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Initialize this enemy's values. Ideally this is only 
    /// called from EnemyManager for spawning purposes
    /// </summary>
    /// <param name="hp">The HP of the spawned enemy</param>
    public void Init(float hp, Vector3 position, Vector3 pitchYawRoll)
    {
        // TRANSFORM data
        transform.SetPositionAndRotation(position, Quaternion.Euler(pitchYawRoll));

        // MATTER STATE data
        debuffState = MatterState.None;
        debuffTime = 0.0f;

        // HP data
        this.hp = hp;
        maxHP = hp;
        transform.GetComponentInChildren<TextMeshPro>().text = hp.ToString();

        // DOT data
        dotTime = 0.0f;
        dotDmg = 0.0f;

        // DEBUFF data
        heatAmt = iceAmt = 0;
        debuffMax = 1.5f;
        waterMoveSpeedMult = 0.9f;

        // STUN/ROOT data
        stunTime = 0.0f;

        // SPEED data
        moveSpeed = GetComponent<NavMeshAgent>().speed;

        // Set active
        gameObject.SetActive(true);
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

        stunTime -= Time.deltaTime;
        agent.speed = stunTime <= 0 ? moveSpeed * (1 - (iceAmt / debuffMax)) : 0;

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
                    debuffTime = seconds;
                    Douse(seconds);
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

    /// <summary>
    /// Stuns the enemy (cannot move) for the given amount of time
    /// </summary>
    /// <param name="seconds">How long to stun for</param>
    public void Stun(float seconds)
    {
        stunTime = seconds;
    }

    // Freezes the enemy, leaving them unresponsive
    public void Freeze()
    {
        debuffState = MatterState.Ice;
        agent.isStopped = true;
        // Nothing yet
    }

    // Burst the enemy, doing single-shot AOE then adding individual DOT effect
    public void Burst(float seconds)
    {
        debuffState = MatterState.Gas;
        
        manager.CreateAOE(transform.position, 4.0f, a =>
        {
            EnemyStats e = a.GetComponent<EnemyStats>();
            e.Stun(1f);
            a.GetComponent<Rigidbody>().AddExplosionForce(5000f, transform.position, 4f);
            e.TakeDamage(35.0f);
        });

        ApplyDOT(50, seconds);
    }

    // Apply any water debuff effects to the enemy
    public void Douse(float seconds)
    {
        debuffState = MatterState.Water;
        agent.speed = moveSpeed * waterMoveSpeedMult;
    }

    // Eliminate enemy debuffs
    public void NeutralizeDebuffs()
    {
        debuffState = MatterState.None;
        agent.isStopped = false;
        agent.speed = moveSpeed;
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

