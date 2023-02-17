using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int poolIndex;

    private float maxHP;
    private MatterState debuffState;
    private float hp;
    private float dotTime;
    private float dotDmg;

    // Start is called before the first frame update
    void Start()
    {
    }

    /// <summary>
    /// Initialize this enemy's values. Ideally this is only 
    /// called from EnemyManager for spawning purposes
    /// </summary>
    /// <param name="hp">The HP of the spawned enemy</param>
    public void Init(float hp, Vector3 position, Vector3 pitchYawRoll, int index)
    {
        // Set transform data
        transform.SetPositionAndRotation(position, Quaternion.Euler(pitchYawRoll));

        debuffState = MatterState.None;
        this.hp = hp;
        maxHP = hp;
        dotTime = 0.0f;
        dotDmg = 0.0f;
        gameObject.SetActive(true);
        poolIndex = index;
    }

    // Update is called once per frame
    void Update()
    {
        dotTime -= Time.deltaTime;
        if (dotTime > 0)
            TakeDamage(dotDmg * Time.deltaTime);
    }

    // Afflict this enemy with a specific Matter debuff: Ice, Water, or Steam
    public void Afflict(MatterState state)
    {
        switch (state)
        {
            case MatterState.Ice:
                // Freeze the enemy if they are currently wet
                if (debuffState == MatterState.Water) 
                    Freeze();
                break;
            case MatterState.Water:
                // Debuff the enemy with Wet, but only if they are not already debuffed
                if (debuffState == MatterState.None) 
                    debuffState = MatterState.Water;
                break;
            case MatterState.Gas:
                // Boil the enemy if they are wet
                if (debuffState == MatterState.Water) 
                    Burst();
                break;
            default: break; // state.None does nothing
        }

        Debug.Log(state.ToString());
    }

    // Freezes the enemy, leaving them unresponsive
    public void Freeze()
    {
        debuffState = MatterState.Ice;
        // Nothing yet
    }

    // Burst the enemy, doing single-shot AOE then adding individual DOT effect
    public void Burst()
    {
        debuffState = MatterState.Gas;

        manager.CreateAOE(transform.position, 4.0f, a => 
            a.GetComponent<EnemyStats>().TakeDamage(35.0f));

        ApplyDOT(50, 4);
    }

    // Eliminate enemy debuffs
    public void NeutralizeDebuffs()
    {
        debuffState = MatterState.None;
    }

    public void TakeDamage(float dmgAmt)
    {
        hp -= dmgAmt;
        Debug.Log($"{hp} HP remaining on {name}");
        if (hp <= 0) manager.KillEnemy(poolIndex);
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

