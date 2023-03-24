using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheckpointData;

public class CheckpointManager : MonoBehaviour
{
    public CheckpointDataObject checkpointData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RespawnAtCheckpoint()
    {
        if (checkpointData == null)
        {
            // this is bad and should never happen. but if it does... who knows
        } else
        {
            // reset player transform
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = checkpointData.playerTransform.position;
            player.transform.rotation = checkpointData.playerTransform.rotation;
            player.transform.localScale = checkpointData.playerTransform.localScale;

            player.GetComponent<PlayerController>().hasGun = checkpointData.playerHasGun;

            // removes unnecessary homes and despawns/respawns all enemies
            GameObject enemyManager = GameObject.FindGameObjectWithTag("EnemyManager");
            enemyManager.GetComponent<EnemyManager>().RemoveHomes(checkpointData.enemyHomesToRemove);
            enemyManager.GetComponent<EnemyManager>().DespawnAllEnemies();
            enemyManager.GetComponent<EnemyManager>().SpawnAllEnemies();

            GameObject[] allInteractables = GameObject.FindGameObjectsWithTag("Interactable");
            foreach (GameObject interactable in allInteractables)
            {
                interactable.GetComponent<Interactable>().Reset();
            }

            for (int i = 0; i < checkpointData.interactablesToActivate.Count; i++)
            {
                checkpointData.interactablesToActivate[i].GetComponent<Interactable>().isActivated = true;
            }

            foreach (GameObject objectToDeactivate in checkpointData.objectsToDeactivate)
            {
                objectToDeactivate.SetActive(false);
            }
        }
    }
}
