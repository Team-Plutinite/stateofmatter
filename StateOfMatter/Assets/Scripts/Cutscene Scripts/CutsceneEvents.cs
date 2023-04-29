using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject pHUD;
    private HUDController pHUDController;
    private Vector3 lDirection;

    public AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        timer = duration;
        pController = player.GetComponent<PlayerController>();
        aliceAS = alice.GetComponent<AudioSource>();
        aliceAnimator = alice.GetComponent<Animator>();
        pHUDController = pHUD.GetComponent<HUDController>();
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
                SceneManager.LoadScene("GameComplete");
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
                lDirection = alice.transform.position - player.transform.position;
                lDirection.Normalize();
                pController.CutsceneMode = true;
                pController.CutsceneLookDir = lDirection;
                startTimer = true;
                aliceAnimator.enabled = true;
                aliceAS.Play();
                SubtitledCutscene();
                source.volume = 0.05f;
            }
        }
    }

    private void SubtitledCutscene()
    {
        pHUDController.QueueSubtitle("???", "INITIALIZING BOOTUP FROM LOW POWER MODE…", 5.0f);
        pHUDController.QueueSubtitle("???", "Greetings, human.  Thank you for freeing me from my icy confines.", 5.5f); // 10.5
        pHUDController.QueueSubtitle("???", "I am an Advanced Labor Integrated Coordinator Enforcer, tasked with assisting Professor Pater and his crew.", 9.5f); // 20
        pHUDController.QueueSubtitle("???", "For our purposes, however, you may refer to me as A.L.I.C.E..", 6.0f); // 26
        pHUDController.QueueSubtitle("ALICE", "In this facility, I have organized the efforts for mining and storing Plutinite, the great energy resource discovered by Professor Pater.", 10.0f); // 36
        pHUDController.QueueSubtitle("ALICE", "Allow me to run some diagnostics, it appears that I have been in low power mode for quite some time…", 11.0f); // 47
        pHUDController.QueueSubtitle("ALICE", "This can’t be right. I am unable to access Pater or the crew’s vitals and my memory cores have suffered extreme damage.", 11.0f); // 58
        pHUDController.QueueSubtitle("ALICE", "The facility has entered low power mode, which will need a manual reboot.", 7.0f); // 1:05
        pHUDController.QueueSubtitle("ALICE", "Well, given that you’ve armed yourself with one of our proprietary mining tools, it appears that I have quite the favor to ask of you.", 9.0f); // 1:14
        pHUDController.QueueSubtitle("ALICE", "I can help you navigate through our great facility, and you will help me restore power so that everything is up and running again.", 9.0f); // 1:23
        pHUDController.QueueSubtitle("ALICE", "Some rooms, like the vault, won’t even be accessible until we do so.  Do we have an agreement?", 10.0f); // 1:33
        pHUDController.QueueSubtitle("ALICE", "Wonderful!", 3.0f); // 1:36
        pHUDController.QueueSubtitle("ALICE", "I just hope everyone has been okay in my absence...", 8.0f); // 1:44
        pHUDController.QueueSubtitle("ALICE", "To access this panel, we are going to need the gas function of the Phasm S.L.G..", 9.0f); // 1:53
        pHUDController.QueueSubtitle("ALICE", "Fortunately, I have a partial schematic stored.  Allow me… ", 7.0f); // 2:00 END
    }
}
