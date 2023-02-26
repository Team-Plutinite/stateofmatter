using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverInteraction : MonoBehaviour
{
    [Tooltip("The door object this lever controls.")]
    public GameObject connectedDoor;
    [Tooltip("Can this lever be deactivated?")]
    public bool deactivatable = false;
    [Tooltip("How close the player must be to interact with this lever.")]
    public float interactDistance = 4;

    private bool activated;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        activated = false;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Press E to interact with this lever
        if (!activated || deactivatable)
        {
            Vector3 playerToLever = transform.position - player.transform.position;
            if (playerToLever.sqrMagnitude < Mathf.Pow(interactDistance, 2))
            {
                Physics.Raycast(player.transform.position,  // Start at player position
                    playerToLever.normalized,               // Raycast toward direction of lever
                    out RaycastHit hit,                     // Store the hit info locally
                    interactDistance,                       // Raycast out by interactDistance
                    6);                                     // layermask 6 = Interactable
                if (hit.collider.gameObject == this)
                {
                    // TODO: show Interact tooltip here


                    // Toggle the door on key interact
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        DoorController doorCtrl = connectedDoor.GetComponent<DoorController>();
                        if (doorCtrl.IsOpen)
                            doorCtrl.Close();
                        else doorCtrl.Open();

                        activated = !activated;
                    }
                }
            }
        }
    }

    public bool Active { get { return activated; } set { activated = value; } }
}
