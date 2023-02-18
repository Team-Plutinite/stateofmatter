using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Tooltip("Enemy's attack damage")]
    public float attackDmg;
    [Tooltip("Enemy's attack cooldown, in seconds")]
    public float attackCooldown;
    [Tooltip("Minimum attack distance between enemy and player")]
    public float minAttackDistance = 1.5f;

    private Navigator navigator;

    private GameObject player;

    private float attackCountdown;

    private IEnumerator coroutine;

    // purely for testing purposes
    private GameObject stick;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        stick = GameObject.Find("Stick");
        attackCountdown = 0f;
        navigator = GetComponent<Navigator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (attackCountdown <= 0f && navigator.distanceToGoal <= minAttackDistance)
        {
            TryAttack();
        }

        attackCountdown -= Time.deltaTime;
    }
    /// <summary>
    /// Attempts an enemy attack on its goal object (the player)
    /// </summary>
    void TryAttack()
    {
        // This stops the enemy's movement and restarts it after the time passed into RestartMovement
        navigator.agent.isStopped = true;

        coroutine = RestartMovement(2.0f);
        StartCoroutine(coroutine);

        coroutine = WaitThenAttack(0.5f);
        StartCoroutine(coroutine);


        attackCountdown = attackCooldown;
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
            player.GetComponent<PlayerStats>().hp -= 1;
        }

        // testing purposes only
        stick.transform.Rotate(0, -30, 0, Space.Self);
    }
}
