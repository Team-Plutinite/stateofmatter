using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MatterState
{
    Ice,
    Water,
    Steam,
    None
}

public class Enemy : MonoBehaviour
{
    public float maxHP;
    public float attackDmg;
    [Tooltip("Enemy's attack cooldown, in seconds")]
    public float attackCD;
    [Tooltip("Enemy movement speed")]
    public float speed;
    [Tooltip("Minimum attack distance between enemy and player")]
    public float minAttackDistance = 1.5f;

    [SerializeField]
    private MatterState debuffState;

    private float hp;
    private float attackCountdown;

    private GameObject player;
    private Navigator navigator;

    private IEnumerator coroutine;

    // purely for testing purposes
    private GameObject stick;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        stick = GameObject.Find("Stick");
        hp = maxHP;
        attackCountdown = 0f;
        navigator = GetComponent<Navigator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0f) Die();
        if (attackCountdown <= 0f && navigator.distanceToGoal <= minAttackDistance) TryAttack();

        attackCountdown -= Time.deltaTime;
    }

    void Die()
    {
        // Nothing yet
    }

    void TryAttack()
    {
        // Nothing yet

        // This stops the enemy's movement and restarts it after the time passed into RestartMovement
        navigator.agent.isStopped = true;
        coroutine = RestartMovement(2.0f);
        StartCoroutine(coroutine);

        coroutine = WaitThenAttack(0.5f);
        StartCoroutine(coroutine);
        

        attackCountdown = 3f;
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

    // Stops enemy movement after given wait time
    // Currently unused, but if its needed in the future for some reason it's there
    IEnumerator StopMovement(float wait)
    {
        yield return new WaitForSeconds(wait);
        navigator.agent.isStopped = true;
    }

    // Restarts enemy movement after given wait time
    IEnumerator RestartMovement(float wait)
    {
        yield return new WaitForSeconds(wait);
        navigator.agent.isStopped = false;
        // testing purposes only
        stick.transform.Rotate(0, 30, 0, Space.Self);
    }

    // Waits a given amount of time, then attacks
    IEnumerator WaitThenAttack(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (navigator.distanceToGoal <= minAttackDistance)
        {
            player.GetComponent<PlayerStats>().hp = player.GetComponent<PlayerStats>().hp - 1;
        }
        
        // testing purposes only
        stick.transform.Rotate(0, -30, 0, Space.Self);
    }
}

