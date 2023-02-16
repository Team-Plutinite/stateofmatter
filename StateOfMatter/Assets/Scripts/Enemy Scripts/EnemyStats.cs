using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MatterState
{
    Ice = 0,
    Water = 1,
    Steam = 2,
    None = 3
}

public class EnemyStats : MonoBehaviour
{
    public float maxHP;
    

    [SerializeField]
    private MatterState debuffState;

    private float hp;
    private Navigator navigator;
    private GameObject player;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        hp = maxHP;
        navigator = GetComponent<Navigator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0f) Die();
    }

    void Die()
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

