using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneEvents : MonoBehaviour
{
    public GameObject player;
    public PlayerController pController;
    private bool startTimer = false;
    public float duration = 120.0f;
    public float timer;
    private bool triggerCutscene = true;

    public GameObject alice;
    private AudioSource aliceAS;
    private Animator aliceAnimator;

    // Start is called before the first frame update
    void Start()
    {
        timer = duration;
        pController = player.GetComponent<PlayerController>();
        aliceAS = alice.GetComponent<AudioSource>();
        aliceAnimator = alice.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                pController.CutsceneMode = false;
                startTimer = false;
                // timer = duration;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerCutscene)
        {
            triggerCutscene = false;
            if (other.gameObject.layer == 6)
            {
                pController.CutsceneMode = true;
                startTimer = true;
                aliceAnimator.enabled = true;
                aliceAS.Play();
            }
        }
    }
}
