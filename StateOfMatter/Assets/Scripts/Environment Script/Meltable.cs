using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Meltable : MonoBehaviour
{

    [SerializeField]
    private float meltHealth = 10.0f;
    [SerializeField]
    private GameObject melter;
    [SerializeField]
    private Vector3 deltaScale = new Vector3(-0.01f, -0.01f, -0.01f);

    private Vector3 testVec = new Vector3(0f, 0f, 0f);


    //As the meltable is hit by gas, it size decreases
    public void Melt(MatterState matterState)
    {
        if (matterState == MatterState.Gas)
        {
            melter.transform.localScale += deltaScale;
        }
        if(matterState == MatterState.Ice)
        {
            melter.transform.localScale -= deltaScale;
        }


    }
}


