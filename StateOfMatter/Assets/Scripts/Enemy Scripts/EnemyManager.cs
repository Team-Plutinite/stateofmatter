using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public delegate void EnemyAction(GameObject e);

public class EnemyManager : MonoBehaviour
{
    private Dictionary<int, GameObject> activeEnemies;
    private Queue<GameObject> inactiveEnemies;
    private List<GameObject> enemyHomes; // currently spawns in at homes; this can be refactored to have spawns and homes seperated. however rn homes = spawns

    [Tooltip("The size of the enemy object pool.")]
    public int poolSize;
    [Tooltip("Reference to the player")]
    public GameObject player;
    [Tooltip("The Prefab to use to instantiate more enemies")]
    public GameObject enemyRef;

    // Start is called before the first frame update
    void Start()
    {
        activeEnemies = new Dictionary<int, GameObject>();
        inactiveEnemies = new Queue<GameObject>();
        enemyHomes = new List<GameObject>();

        // Create the enemy pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newEnemy = Instantiate(enemyRef);
            newEnemy.GetComponent<EnemyStats>().manager = this;
            newEnemy.SetActive(false);
            inactiveEnemies.Enqueue(newEnemy);
        }

        // Create list of enemy homes
        enemyHomes.AddRange(GameObject.FindGameObjectsWithTag("EnemyHome"));

        // Spawn enemies based on homes

        for (int i = 0; i < enemyHomes.Count; i++)
        {
            SpawnEnemy(100, enemyHomes[i].transform.position, Vector3.zero, player, enemyHomes[i].transform);
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
        List<GameObject> inRange = new List<GameObject>();

        // Fill a list of all objects in range (doing this so we aren't directly modifying activeEnemies
        foreach (GameObject e in activeEnemies.Values)
        {
            if ((e.transform.position - position).sqrMagnitude < radius * radius)
                inRange.Add(e);
        }

        // Invoke action on objects in range (start at end because some may be removed in this process)
        for (int i = inRange.Count - 1; i >= 0; i--)
            action(inRange[i]);
    }

    /// <summary>
    /// Spawns an enemy with a designated rotation and orientation
    /// </summary>
    /// <param name="hp">The HP of the enemy</param>
    /// <param name="transform">The enemy's transform</param>
    /// <returns>If spawning was successful</returns>
    public bool SpawnEnemy(float hp, Vector3 position, Vector3 pitchYawRoll, GameObject goal = null, Transform home = null)
    {
        
        // Attempt to dequeue from inactive pool; if pool is empty, return false
        if (!inactiveEnemies.TryDequeue(out GameObject enemy))
            return false;

        if (goal == null && player != null)
        {
            goal = player;
        }
        // Add it to the active pool
        activeEnemies.Add(enemy.GetInstanceID(), enemy);

        // Initialize the enemy (spawning it). Also store its pool index
        enemy.GetComponent<EnemyStats>().Init(hp, position, pitchYawRoll);
        // Init everything in the EnemyAttack component
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
    public bool KillEnemy(int instanceID)
    {
        // Make sure we aren't OOBing.
        if (!activeEnemies.ContainsKey(instanceID)) 
            return false;

        // Move enemy from active to inactive, deactivate it
        GameObject enemy = activeEnemies[instanceID];
        activeEnemies.Remove(instanceID);
        enemy.SetActive(false);
        inactiveEnemies.Enqueue(enemy);
        return true;
    }

    public void RemoveHomes(List<GameObject> homesToRemove)
    {
        enemyHomes = enemyHomes.Except(homesToRemove).ToList();
    }

    public void DespawnAllEnemies()
    {
        GameObject[] enemyArr = new GameObject[Enemies.Count];
        Enemies.Values.CopyTo(enemyArr, 0);
        for (int i = 0; i < enemyArr.Length; i++)
        {
            enemyArr[i].GetComponent<EnemyStats>().TakeDamage(999);
        }
    }

    public void SpawnAllEnemies()
    {
        for (int i = 0; i < enemyHomes.Count; i++)
        {
            SpawnEnemy(100, enemyHomes[i].transform.position, Vector3.zero, player, enemyHomes[i].transform);
        }
    }

    /// <summary>
    /// Returns the pool of all active (alive) enemies in the game world.
    /// </summary>
    public Dictionary<int, GameObject> Enemies { get { return activeEnemies; } }
}
