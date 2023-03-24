using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheckpointData;

namespace CheckpointData
{
    public class CheckpointDataObject
    {
        public Transform playerTransform;
        public bool playerHasGun;
        public List<GameObject> enemyHomesToRemove;
        // objectsToDeactivate are all just setActive(false)
        // for example: meltables that have to be melted, gun prop
        public List<GameObject> objectsToDeactivate;
        public List<GameObject> interactablesToActivate;

        public CheckpointDataObject()
        {

        }

        public CheckpointDataObject(List<GameObject> _enemyHomesToRemove, List<GameObject> _objectsToDeactivate, List<GameObject> _interactablesToActivate, Transform _playerTransform, bool _playerHasGun)
        {
            enemyHomesToRemove = _enemyHomesToRemove;
            objectsToDeactivate = _objectsToDeactivate;
            interactablesToActivate = _interactablesToActivate;
            playerTransform = _playerTransform;
            playerHasGun = _playerHasGun;
        }
    }
}

public class Checkpoint : MonoBehaviour
{
    public Transform respawnTransform;
    public bool playerHasGun = true;
    public List<GameObject> enemyHomesToRemove;
    public List<GameObject> objectsToDeactivate;
    public List<GameObject> interactablesToActivate;

    public GameObject checkpointManager;

    //private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (respawnTransform == null)
            {
                respawnTransform = this.transform;
            }

            CheckpointDataObject checkpointData = new CheckpointDataObject(enemyHomesToRemove, objectsToDeactivate, interactablesToActivate, respawnTransform, playerHasGun);
            checkpointManager.GetComponent<CheckpointManager>().checkpointData = checkpointData;
            

        }
    }


}
