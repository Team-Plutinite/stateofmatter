using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatterState
{
    Ice = 0,
    Water = 1,
    Steam = 2,
    None = 3
}

public class Enemy : MonoBehaviour
{
    public float maxHP;
    public float attackDmg;
    [Tooltip("Enemy's attack cooldown, in seconds")]
    public float attackCD;
    [Tooltip("Enemy movement speed")]
    public float speed;

    [SerializeField]
    private MatterState debuffState;

    private float hp;
    private float attackCountdown;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        hp = maxHP;
        attackCountdown = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0f) Die();
        if (attackCountdown <= 0f) TryAttack();

        attackCountdown -= Time.deltaTime;
    }

    void Die()
    {
        // Nothing yet
    }

    void TryAttack()
    {
        // Nothing yet
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
            case MatterState.Steam:
                // Boil the enemy if they are wet
                if (debuffState == MatterState.Water) 
                    Boil();
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

    // Boil the enemy, adding a DOT effect
    public void Boil()
    {
        debuffState = MatterState.Steam;
        // Nothing yet
    }

    // Eliminate enemy debuffs
    public void NeutralizeDebuffs()
    {
        debuffState = MatterState.None;
    }
}
