using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour, IInteractable
{
    public GameObject playerGun; // Reference to the gun on the player

    public void Activate()
    {
        gameObject.SetActive(false);
        // play equipping animation
        // play some sort of sound
        // etc etc etc...
        playerGun.SetActive(true);
        
    }
}
