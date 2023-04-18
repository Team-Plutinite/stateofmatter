using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PressurePlate : MonoBehaviour
{
    [SerializeField] // serialized for debug purposes
    private bool pressed = false;

    public AudioSource source;
    public AudioClip stepOnSound;
    public AudioClip stepOffSound;

    void Start()
    {
        source.volume = 0.3f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy" || other.gameObject.tag == "Weight")
        {
            this.pressed = true;
            //gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f,gameObject.transform.position.z);

            source.PlayOneShot(stepOnSound);
            Debug.Log(pressed + " On Plate");
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy" || other.gameObject.tag == "Weight")
        {
            this.pressed = false;
            //gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z);

            source.PlayOneShot(stepOffSound);
            Debug.Log(pressed + " Off Plate");            
        }

    }

    public bool Pressed
    {
        get { return pressed; }
        set { pressed = value; }
    }
}
