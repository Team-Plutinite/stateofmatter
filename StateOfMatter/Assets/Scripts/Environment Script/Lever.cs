using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField]
    private GameObject ice;
    [SerializeField]
    private GameObject leverArm;
    private Animation animationComponent;

    // Start is called before the first frame update
    void Start()
    {
        animationComponent = leverArm.GetComponent<Animation>();
        animationComponent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        if (!animationComponent.enabled)
        {
            return;
        } else
        {
            animationComponent.enabled = true;
            Debug.Log("playing animation");
            animationComponent.Play("Activate");
            animationComponent.enabled = false;
        }
    }
}
