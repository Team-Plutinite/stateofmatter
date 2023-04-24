using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MeltDirection //Used to determine the direction to melt the meltable/
{
    X,
    Y,
    Z,
    All
}

public class Meltable : MonoBehaviour
{

    [SerializeField]
    private GameObject melter;
    [SerializeField]
    private Vector3 deltaScaleAll = new Vector3(-0.01f, -0.01f, -0.01f);

    [SerializeField]
    private MeltDirection direction = MeltDirection.All;

    private Vector3 testVec = new Vector3(0f, 0f, 0f);

    //Determines which axis the meltable melts from
    private Dictionary<MeltDirection, Vector3> meltDirectionDic = new Dictionary<MeltDirection, Vector3>();

    // audio
    public AudioSource source;
    public AudioClip meltingSound;
    private float meltSoundTimer;
    private const float meltSoundCooldown = 0.145f;

    private void Awake()
    {
        meltDirectionDic.Add(MeltDirection.X, new Vector3(deltaScaleAll.x, 0f, 0f));
        meltDirectionDic.Add(MeltDirection.Y, new Vector3( 0f, deltaScaleAll.y, 0f));
        meltDirectionDic.Add(MeltDirection.Z, new Vector3(0f, 0f, deltaScaleAll.z));
        meltDirectionDic.Add(MeltDirection.All, deltaScaleAll);

        source = gameObject.AddComponent<AudioSource>();
        source.volume = 0.2f;
        meltSoundTimer = 0.0f;
    }

    void Update()
    {
        meltSoundTimer -= Time.deltaTime;
    }

    public GameObject GetMelter()
    {
        return melter;
    }
    public void SetMelter(GameObject value)
    {
        melter = value;
    }

   
    //As the meltable is hit by gas, it size decreases
    public void Melt(MatterState matterState)
    {
        if (matterState == MatterState.Gas)
        {
            if (meltSoundTimer <= 0.0f)
            {
                source.PlayOneShot(meltingSound);
                Debug.Log("melting sound playing");
                meltSoundTimer = meltSoundCooldown;
            }

            melter.transform.localScale += meltDirectionDic[direction];
            if (melter.transform.localScale.x <= 0 || melter.transform.localScale.y <= 0 || melter.transform.localScale.z <= 0)
            {
                melter.gameObject.SetActive(false);
            }
            if(direction != MeltDirection.All)
            {
                melter.transform.localPosition += meltDirectionDic[direction] / 2;
            }
            
        }
        if(matterState == MatterState.Ice)
        {
            if (melter.transform.localScale.x <= 1f && melter.transform.localScale.y <= 1f && melter.transform.localScale.z <= 1f)
            {
                melter.transform.localScale -= meltDirectionDic[direction];
                //waterState.SetActive(false);
            }
            else
            {
                return;
            }
            if (direction != MeltDirection.All)
            {
                melter.transform.localPosition -= meltDirectionDic[direction] / 2;
            }
            
        }


    }
}


