using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardButton : MonoBehaviour, IInteractable
{
    [SerializeField]
    SecurityFeed feed;

    public AudioSource source;
    [SerializeField]
    public AudioClip buttonSound;

    void Start()
    {
        source.volume = 0.2f;
    }

    void Update()
    {
        
    }

    public void Activate()
    {
        feed.BackButton = false;
        source.PlayOneShot(buttonSound);
    }

    public void Deactivate()
    {
        
    }
}
