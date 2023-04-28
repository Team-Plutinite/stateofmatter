using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    public float attackDmg;
    [Tooltip("Enemy's attack cooldown, in seconds")]
    public float attackCD = 2.0f;
    [Tooltip("Enemy movement speed")]
    public float speed;
    [Tooltip("Minimum attack distance between enemy and player")]
    public float minAttackDistance = 1.5f;

    private Navigator navigator;

    private GameObject player;

    private float attackCountdown;

    private IEnumerator coroutine;

    public AudioSource source;
    public AudioClip attackSound;

    // purely for testing purposes
    //private GameObject stick;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        //stick = GameObject.Find("Stick");
        attackCountdown = 0f;
        navigator = GetComponent<Navigator>();

        source.volume = 0.4f;
    }


    void Update()
    {
        if (attackCountdown <= 0f && navigator.distanceToGoal <= minAttackDistance) TryAttack();

        attackCountdown -= Time.deltaTime;
    }

    void TryAttack()
    {
        // This stops the enemy's movement and restarts it after the time passed into RestartMovement
        navigator.agent.isStopped = true;
        coroutine = RestartMovement(attackCD);
        StartCoroutine(coroutine);

        coroutine = WaitThenAttack(0.5f);
        StartCoroutine(coroutine);

        source.PlayOneShot(attackSound);
        attackCountdown = 3f;
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
        //stick.transform.Rotate(0, 30, 0, Space.Self);
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
        //stick.transform.Rotate(0, -30, 0, Space.Self);
    }
}
