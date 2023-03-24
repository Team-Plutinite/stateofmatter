using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityFeed : MonoBehaviour, IInteractable
{
    [SerializeField]
    private List<Camera> securityCams;

    [SerializeField]
    private RenderTexture renderTexture;

    [SerializeField]
    public bool backButton;

    private Interactable interactableComponent;

    private int idx;

    // Start is called before the first frame update
    void Start()
    {

        interactableComponent = gameObject.GetComponent<Interactable>();
        interactableComponent.isInteractable = true;
        idx = 0;
        securityCams[idx].targetTexture = renderTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {

        if (backButton)
        {
            if(idx - 1 < 0)
            {
                return;
            }
            else
            {

                securityCams[idx].targetTexture = null ;
                idx--;
                securityCams[idx].targetTexture = renderTexture;
                
            }
            
        }
        if (!backButton)
        {
            if (idx + 1 >= securityCams.Count)
            {
                return;
            }
            else
            {

                securityCams[idx].targetTexture = null;
                idx++;
                securityCams[idx].targetTexture = renderTexture;
            }
        }
    }


}
