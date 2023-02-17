using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EnemyAction(GameObject e);

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> activeEnemies;
    private Queue<GameObject> inactiveEnemies;

    [Tooltip("The size of the enemy object pool.")]
    public int poolSize;
    [Tooltip("Reference to the player")]
    public GameObject player;
    [Tooltip("The Prefab to use to instantiate more enemies")]
    public GameObject enemyRef;

    // Start is called before the first frame update
    void Start()
    {
        activeEnemies = new List<GameObject>();
        inactiveEnemies = new Queue<GameObject>();

        // Create the enemy pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newEnemy = Instantiate(enemyRef);
            newEnemy.GetComponent<EnemyStats>().manager = this;
            newEnemy.SetActive(false);
            inactiveEnemies.Enqueue(newEnemy);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Invoke a particular action on all enemies within an area of effect
    /// </summary>
    /// <param name="position">The position of the effect</param>
    /// <param name="radius">The radius of the effect</param>
    /// <param name="action">The function to perform on each enemy in the AoE</param>
    public void CreateAOE(Vector3 position, float radius, EnemyAction action)
    {
        foreach (GameObject e in activeEnemies)
        {
            if ((e.transform.position - position).sqrMagnitude < radius * radius)
                action(e);
        }
    }

    /// <summary>
    /// Spawns an enemy with a designated rotation and orientation
    /// </summary>
    /// <param name="hp">The HP of the enemy</param>
    /// <param name="transform">The enemy's transform</param>
    /// <returns>If spawning was successful</returns>
    public bool SpawnEnemy(float hp, Vector3 position, Vector3 pitchYawRoll, Transform goal = null, Transform home = null)
    {
        // Attempt to dequeue from inactive pool; if pool is empty, return false
        if (!inactiveEnemies.TryDequeue(out GameObject enemy))
            return false;

        // Add it to the active pool
        activeEnemies.Add(enemy);

        // Initialize the enemy (spawning it). Also store its pool index
        enemy.GetComponent<EnemyStats>().Init(hp, position, pitchYawRoll, activeEnemies.Count - 1);
        // Init everything in the EnemyAttack compoenent
        //enemy.GetComponent<EnemyAttack>().Init();
        // Init everything in the Navigator component
        enemy.GetComponent<Navigator>().Init(goal, home);

        return true;
    }

    /// <summary>
    /// Kill the specified enemy (this should only be called from EnemyStats)
    /// </summary>
    /// <param name="index">The enemy's index within the active enemy pool</param>
    /// <returns>Whether killing the enemy was successful or not</returns>
    public bool KillEnemy(int index)
    {
        // Make sure we aren't OOBing.
        if (index < 0 || index >= activeEnemies.Count) 
            return false;

        // Remove the enemy from active pool
        GameObject enemy = activeEnemies[index];
        activeEnemies.RemoveAt(index);

        // Deactivate it and enqueue it in the inactive pool
        enemy.SetActive(false);
        inactiveEnemies.Enqueue(enemy);

        return true;
    }

    /// <summary>
    /// Returns the pool of all active (alive) enemies in the game world.
    /// </summary>
    public List<GameObject> Enemies { get { return activeEnemies; } }
}
