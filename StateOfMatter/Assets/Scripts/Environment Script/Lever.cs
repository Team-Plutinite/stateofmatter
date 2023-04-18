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

    public Material LeverOffMaterial;
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

    public void Deactivate()
    {
        // reset animations
        animatorComponent.Rebind();
        animatorComponent.Update(0f);
        // turn material to off material
        lever.GetComponent<MeshRenderer>().material = LeverOffMaterial;
        // bring back ice
        if (!ice.gameObject.activeSelf)
        {
            ice.gameObject.SetActive(true);
        }

    }
}
