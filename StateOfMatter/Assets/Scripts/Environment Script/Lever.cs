using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField]
    private GameObject ice;
    [SerializeField]
    private GameObject lever;
    private Animator animatorComponent;
    private Interactable interactableComponent;

    public Material LeverOnMaterial;

    // Start is called before the first frame update
    void Start()
    {
        animatorComponent = lever.GetComponent<Animator>();
        interactableComponent = gameObject.GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ice.gameObject.activeSelf)
        {
            interactableComponent.isInteractable = true;
        }
    }

    public void Activate()
    {
        animatorComponent.SetTrigger("Activate");

        lever.GetComponent<MeshRenderer>().material = LeverOnMaterial;
    }
}
