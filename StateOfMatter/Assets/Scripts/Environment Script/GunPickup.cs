using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GameObject playerGun; // Reference to the gun on the player
    public GameObject player;


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            gameObject.SetActive(false);
            // play equipping animation
            // play some sort of sound
            // etc etc etc...
            player.GetComponent<PlayerController>().hasGun = true;
        }
    }
}
