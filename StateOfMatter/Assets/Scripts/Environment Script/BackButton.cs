using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour, IInteractable
{

    [SerializeField]
    SecurityFeed feed;

    public AudioSource source;
    [SerializeField]
    public AudioClip buttonSound;

    void Start()
    {
        source.volume = 0.3f;
    }

    public void Activate()
    {
        feed.BackButton = true;
        source.PlayOneShot(buttonSound);
    }

    public void Deactivate()
    {
        
    }
}
