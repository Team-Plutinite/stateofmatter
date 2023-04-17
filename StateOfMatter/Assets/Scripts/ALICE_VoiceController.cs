using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALICE_VoiceController : MonoBehaviour
{
    private AudioSource dialogue;
    float[] spectrumData = new float[1024];
    [SerializeField]
    [Range(0f, 1000f)]
    float mutiplier = 25f;

    private Light voiceLight;

    // Start is called before the first frame update
    void Start()
    {
        dialogue = GetComponent<AudioSource>();
        voiceLight = transform.GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        dialogue.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        voiceLight.intensity = Mathf.Max(spectrumData) * mutiplier;
    }
}
