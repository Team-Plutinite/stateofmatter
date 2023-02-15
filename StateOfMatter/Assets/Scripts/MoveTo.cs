using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    
    private NavMeshAgent agent;
    [Tooltip("Player object (for traveling to)")]
    public Transform player;
    [Tooltip("Enemy's 'home' it should return to if player not detected")]
    public Transform home;
    [Tooltip("Minimum distance for player detection")]
    public float detectionDistance;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.destination = goal.position;
    }

    private void FixedUpdate()
    {
        
    }

    void Update()
    {
        // yes this code is ugly. it will be better once the level is built and I can build the enemy a bit more around the level I promise
        if (home != null)
        {
            if (Vector3.Distance(player.position, transform.position) <= detectionDistance)
            {
                agent.destination = player.position;
            }
            else
            {
                agent.destination = home.position;
            }
        } else
        {
            if (Vector3.Distance(player.position, transform.position) <= detectionDistance)
            {
                agent.destination = player.position;
            }
        }
    }
}
