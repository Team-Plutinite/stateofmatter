using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardButton : MonoBehaviour, IInteractable
{
    [SerializeField]
    SecurityFeed feed;

   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        feed.BackButton = false;
    }

    public void Deactivate()
    {
        
    }
}
