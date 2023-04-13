using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDoor : MonoBehaviour, IActivatable
{
    private Animator animatorComponent;

    public AudioSource source;
    public AudioClip doorOpenSound;
    void Start()
    {
        animatorComponent = gameObject.GetComponent<Animator>();

        source = gameObject.AddComponent<AudioSource>();
        source.volume = 0.3f;
    }

    public void Activate()
    {
        animatorComponent.SetTrigger("Activate");

        source.PlayOneShot(doorOpenSound);
    }

    public void Deactivate()
    {
        animatorComponent.SetTrigger("Deactivate");
    }
}
