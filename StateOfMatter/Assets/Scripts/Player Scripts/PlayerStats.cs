using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHP;
    public float hp;

    public GameObject checkpointManager;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0f) Die();
    }

    void Die()
    {
        checkpointManager.GetComponent<CheckpointManager>().RespawnAtCheckpoint();
    }
}
