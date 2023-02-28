using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climber : MonoBehaviour
{
    [Tooltip("How fast the player is able to climb up this object.")]
    public float climbSpeed = 5.0f;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            player.GetComponent<Rigidbody>().useGravity = false;
            player.GetComponent<PlayerController>().OnLadder = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            Rigidbody body = player.GetComponent<Rigidbody>();
            PlayerController pctrl = player.GetComponent<PlayerController>();
            float yVel = 0;

            // If the player is moving into the ladder, lift them up
            if (pctrl.InputDirection.sqrMagnitude > 0 && 
                Vector3.Dot((transform.position - player.transform.position).normalized, 
                pctrl.InputDirection) > 0)
            {
                yVel = 200.0f;
            }
            body.velocity = new(
                    body.velocity.x,
                    yVel * Time.deltaTime,
                    body.velocity.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
            player.GetComponent<PlayerController>().OnLadder = false;
    }
}
