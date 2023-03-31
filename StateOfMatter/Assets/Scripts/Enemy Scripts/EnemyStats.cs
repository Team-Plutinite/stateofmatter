using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    private float solidMoveSpeedMult;

    private float maxHP;
    private float hp;

    private MatterState debuffState;
    private float debuffMax;

    private float heatAmt, iceAmt;

    private float moveSpeed;

    // component references
    private NavMeshAgent agent;
    private Rigidbody body;

    Dictionary<string, Debuff> debuffMap;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        body = GetComponent<Rigidbody>();
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

        // HP data
        this.hp = hp;
        maxHP = hp;
        transform.GetComponentInChildren<TextMeshPro>().text = hp.ToString();

        // DEBUFF data
        heatAmt = iceAmt = 0;
        debuffMax = 1.0f;
        solidMoveSpeedMult = 0.8f;

        // SPEED data
        moveSpeed = GetComponent<NavMeshAgent>().speed;

        // Set active
        gameObject.SetActive(true);

        // reset debuff map
        debuffMap = new Dictionary<string, Debuff>();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle all debuffs
        string[] keys = new string[debuffMap.Count];
        debuffMap.Keys.CopyTo(keys, 0);

        foreach (string key in keys)
        {
            if (debuffMap[key].Timer > 0.0f)
                debuffMap[key].Tick();
            else
            {
                debuffMap[key].InvokeFinalAction();
                debuffMap.Remove(key);
            }
        }

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
    public void Afflict(MatterState state, float seconds, float debuffAmt = 0.0f)
    {
        switch (state)
        {
            case MatterState.Ice:
                debuffMap["RAW_SOLID_DEBUFF"] = new Debuff(seconds,
                    () => agent.speed = moveSpeed * solidMoveSpeedMult * (1 - (iceAmt / debuffMax)), null, NeutralizeDebuffs);
                
                // Freeze the enemy if they are currently wet
                if (debuffState == MatterState.Water)
                {
                    debuffMap["STATE_DEBUFF"].ResetTimer();
                    iceAmt += debuffAmt;
                    if (iceAmt >= debuffMax)
                    {
                        debuffMap.Remove("RAW_SOLID_DEBUFF");
                        debuffMap["STATE_DEBUFF"] = new Debuff(seconds, Freeze, null, NeutralizeDebuffs);
                    }
                }  
                break;

            case MatterState.Water:
                // Debuff the enemy with Wet, but only if they are not already debuffed
                if (debuffState == MatterState.None || debuffState == MatterState.Water)
                {
                    if (!debuffMap.ContainsKey("STATE_DEBUFF"))
                    {
                        debuffMap["STATE_DEBUFF"] = new Debuff(seconds, 
                            () => debuffState = MatterState.Water, 
                            null, 
                            () => { if (debuffState != MatterState.None) NeutralizeDebuffs(); });
                    }
                    else
                        debuffMap["STATE_DEBUFF"].ResetTimer();
                }
                break;

            case MatterState.Gas:
                // Burst the enemy if they are wet
                if (debuffState == MatterState.Water)
                {
                    debuffMap["STATE_DEBUFF"].ResetTimer();
                    heatAmt += debuffAmt;
                    if (heatAmt >= debuffMax)
                        debuffMap["STATE_DEBUFF"] = new Debuff(seconds, Burst, null, NeutralizeDebuffs);
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
        debuffMap["STUN"] = new Debuff(seconds, () => agent.isStopped = true, null, () => agent.isStopped = false);
    }

    // Freezes the enemy, leaving them unresponsive
    public void Freeze()
    {
        iceAmt = 0;
        debuffState = MatterState.Ice;
        agent.isStopped = true;
    }

    // Burst the enemy, doing single-shot AOE then adding individual DOT effect
    public void Burst()
    {
        heatAmt = 0;
        debuffState = MatterState.Gas;
        
        manager.CreateAOE(transform.position, 3.0f, a =>
        {
            EnemyStats e = a.GetComponent<EnemyStats>();
            e.Stun(0.75f);
            a.GetComponent<Rigidbody>().AddExplosionForce(3000f, transform.position, 3f);
            e.TakeDamage(30.0f);
        });
    }

    /// <summary>
    /// If this enemy is currently FROZEN, this will deal a massive amt of dmg
    /// while also unfreezing it.
    /// </summary>
    /// <returns>If shatter attempt was successful</returns>
    public bool Shatter()
    {
        // Big shatter dmg
        if (debuffState == MatterState.Ice)
        {
            TakeDamage(100.0f);
            NeutralizeDebuffs();
            return true;
        }
        return false;
    }

    // Eliminate enemy debuffs
    public void NeutralizeDebuffs()
    {
        debuffState = MatterState.None;
        agent.isStopped = false;
        agent.speed = moveSpeed;
        heatAmt = 0;
        iceAmt = 0;
    }

    public void TakeDamage(float dmgAmt)
    {
        hp -= dmgAmt;
        transform.GetComponentInChildren<TextMeshPro>().text = hp.ToString();

        if (hp <= 0) manager.KillEnemy(gameObject.GetInstanceID());
    }

    /// <summary>
    /// Applies a debuff to the enemy.
    /// </summary>
    /// <param name="debuffName">The name of the debuff in the debuff map. 
    /// BE CAREFUL WITH THIS - if you're trying to create an entirely new debuff, use a unique name!</param>
    /// <param name="debuff">The debuff object to apply.</param>
    public void ApplyDebuff(string debuffName, Debuff debuff)
    {
        debuffMap[debuffName] = debuff;
    }

    /// <summary>
    /// Resets the timer of the specified debuff.
    /// </summary>
    /// <param name="debuffName">The name of the debuff to reset.</param>
    /// <returns>True if the debuff exists.</returns>
    public bool ResetDebuff(string debuffName)
    {
        if (!debuffMap.ContainsKey(debuffName)) 
            return false;

        debuffMap[debuffName].ResetTimer();
        return true;
    }

    /// <summary>
    /// Returns the debuff object corresponding to the specified name.
    /// </summary>
    /// <param name="debuffName">The name of the debuff.</param>
    /// <returns>The corresponding debuff, or null if this name does not exist in the debuff map.</returns>
    public Debuff GetDebuff(string debuffName)
    {
        return debuffMap[debuffName];
    }

    /// <summary>
    /// Gets teh names of all currently active debuffs on this enemy.
    /// </summary>
    /// <returns>A string array of the names of all active debuffs.</returns>
    public string[] GetAllDebuffNames()
    {
        string[] names = new string[debuffMap.Count];
        debuffMap.Keys.CopyTo(names, 0);
        return names;
    }
}