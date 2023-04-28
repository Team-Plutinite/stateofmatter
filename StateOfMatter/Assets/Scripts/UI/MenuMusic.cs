using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AudioSource source;
    public AudioClip track2;
    void Start()
    {
        source.volume = 0.25f;
    }

    void Update()
    {
        if (source.isPlaying == false)
        {
            Debug.Log("changed track");
            source.PlayOneShot(track2);
            source.loop = true;
        }
    }
}
