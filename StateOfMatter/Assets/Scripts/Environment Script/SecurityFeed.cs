using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityFeed : MonoBehaviour, IActivatable
{
    [SerializeField]
    private List<Camera> securityCams;

    [SerializeField]
    private RenderTexture renderTexture;

    [SerializeField]
    bool backButt;


    public bool BackButton
    {
        get { return backButt; }
        set { backButt = value; }
    }

    private int idx;

    // Start is called before the first frame update
    void Start()
    {
        idx = 0;
        securityCams[idx].targetTexture = renderTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        securityCams[idx].targetTexture = null;

        if (backButt)
        {
            //If statement handles rolling over the camera feed.
            if (idx - 1 < 0)
            {
                idx = securityCams.Count;
            }
          
            idx--;
            securityCams[idx].targetTexture = renderTexture;

               
            
            
        }
        if (!backButt)
        {

            //If statement handles rolling over the camera feed.
            if (idx + 1 >= securityCams.Count)
            {
                idx = -1;
            }
            
            
                
            idx++;
            securityCams[idx].targetTexture = renderTexture;
            
        }

        Debug.Log(idx);
    }

    public void Deactivate()
    {
    }
}
