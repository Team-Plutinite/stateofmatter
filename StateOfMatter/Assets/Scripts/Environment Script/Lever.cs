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
    private float soundTimer = 0.5f;
    private bool startCheck = false;

    void Start()
    {
        animatorComponent = lever.GetComponent<Animator>();
        interactableComponent = gameObject.GetComponent<Interactable>();

        source.volume = 0.3f;
    }

    void Update()
    {
        if (!ice.gameObject.activeSelf)
        {
            interactableComponent.isInteractable = true;
        }
        soundTimer -= Time.deltaTime;
        if (soundTimer <= 0)
            startCheck = true;
    }

    public void Activate()
    {
        animatorComponent.SetTrigger("Activate");

        lever.GetComponent<MeshRenderer>().material = LeverOnMaterial;

        if(startCheck) //levers trigger this at start of level, this ensures sound won't play upon level start
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
