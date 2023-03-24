using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField]
    private GameObject ice;
    [SerializeField]
    private GameObject lever;
    private Animator animatorComponent;
    private Interactable interactableComponent;

    public Material LeverOnMaterial;

    public AudioSource source;
    public AudioClip leverSound;

    void Start()
    {
        animatorComponent = lever.GetComponent<Animator>();
        interactableComponent = gameObject.GetComponent<Interactable>();

        source = gameObject.AddComponent<AudioSource>();
        source.volume = 0.3f;
    }

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

        source.PlayOneShot(leverSound);
    }
}
