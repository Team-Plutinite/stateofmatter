using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI;
using UnityEditor;
using UnityEditor.UIElements;

public class Navigator : MonoBehaviour
{
    [Tooltip("Goal object (transform) to travel to")]
    public Transform goal;
    [Tooltip("Enable or disable movement")]
    public bool movementEnabled = false;
    [Header("Detection Settings")]
    [Tooltip("Enable if it only move if its goal is within the detection distance")]
    public bool requiresDetection = true;
    // If for some reason it is felt necessary I'll go through and make a custom editor window to gray these two out if requiresDetection is false
    [Tooltip("Minimum distance between the goal and the object")]
    [MinAttribute(0)]
    public float detectionDistance;
    [Tooltip("Closest the object can get to the goal. If within this range, it will stop moving")]
    [MinAttribute(0)]
    public float maxCloseness = 1;
    [Tooltip("Object's 'home' (transform) it should return to if goal not within detection distance. If null, it will stop in place until the goal re-enters detection distance")]
    public Transform home;
    [Tooltip("Enable if movement should be enabled on line of sight with the goal. Only use if movement is disabled by default")]
    public bool activateOnSight = true;

    [HideInInspector]
    public float distanceToGoal;
    [HideInInspector]
    public NavMeshAgent agent;
    
    private NavMeshHit hit;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (activateOnSight) {
            movementEnabled = false;
        }

    }

    // Called by EnemyStats.Init() when enemy is spawned
    public void Init(Transform goal, Transform home, bool movementEnabled = false, bool requiresDetection = false, float maxCloseness = 1, bool activateOnSight = true)
    {
        this.goal = goal;
        this.movementEnabled = movementEnabled;
        this.requiresDetection = requiresDetection;
        this.maxCloseness = maxCloseness;
        this.home = home;
        this.activateOnSight = activateOnSight;
    }

    // When the agent's destination (agent.destination) is set, the agent will pathfind to that destination. Once it reaches it, it stops.

    void Update()
    {
        if (goal != null)
        {
            distanceToGoal = Vector3.Distance(goal.position, transform.position);

            // If the object requires line of sight with the goal to activate, it will be inactive until it has LOS
            if (activateOnSight && movementEnabled == false)
            {
                // Currently, this does not actually use what direction the object is facing. It just draws a ray from object-goal and if it's unobstructed it's good.
                // Down the line once there's art/models/a level/etc. and not just a red cylinder this can be switched to physics raycasting in a certain direction(s)
                if (!agent.Raycast(goal.position, out hit))
                {
                    movementEnabled = true;
                }
            }
            else if (activateOnSight == false && movementEnabled == false)
            {
                movementEnabled = true;
            }

            if (movementEnabled)
            {

                // If the pathfinding object requires detection it checks if the goal is within range, and if so, moves to it.
                // If it has a home set, it will return to that home when the goal leaves its range
                // If it doesn't require detection it will continue pathfinding to the goal forever
                // Will we actually use the home for enemies? Unknown but I know Mario 64 has it so it might be useful down the line.
                if (requiresDetection)
                {
                    if (distanceToGoal <= detectionDistance)
                    {
                        agent.destination = goal.position;
                    }
                    else if (home != null && transform.position != home.position)
                    {
                        agent.destination = home.position;
                    }
                }
                else
                {
                    agent.destination = goal.position;
                }

                if (distanceToGoal <= maxCloseness)
                {
                    agent.destination = transform.position;
                }
            }
        }
    }
}
