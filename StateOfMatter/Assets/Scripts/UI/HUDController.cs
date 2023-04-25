using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    private struct Dialogue
    {
        public Dialogue(string speaker, string message, float time)
        {
            this.speaker = speaker;
            this.message = message;
            this.time = time;
        }
        public string speaker;
        public string message;
        public float time;
    }
    // Gas mode sprites
    private Image indicatorLeft;
    private Image indicatorRight;
    private Image gasCrossRed;

    private GameObject[] hudStateSprites;
    private GameObject[] hudCrosshairSprites;

    [SerializeField] private GameObject hudSubtitle;
    [SerializeField] private GameObject hudHelmet;
    [SerializeField] private GameObject hudLines;

    private Queue<Dialogue> dialogueQ;
    private Dialogue currentDialogue;

    // Start is called before the first frame update
    void Start()
    {
        // Set references
        indicatorLeft = transform.Find("HUDCanvas/CrosshairSprites/GasCrossImg/ChargeUp_Indicator_Left").GetComponent<Image>();
        indicatorRight = transform.Find("HUDCanvas/CrosshairSprites/GasCrossImg/ChargeUp_Indicator_Right").GetComponent<Image>();
        gasCrossRed = transform.Find("HUDCanvas/CrosshairSprites/GasCrossImg/GasCrossImgRed").GetComponent<Image>();
        if (indicatorLeft == null) Debug.Log("ERROR in HUDController: left indicator image was not found.");
        if (indicatorRight == null) Debug.Log("ERROR in HUDController: right indicator image was not found.");

        hudStateSprites = new GameObject[3];
        hudCrosshairSprites = new GameObject[4];

        hudStateSprites[0] = transform.Find("HUDCanvas/StateSprites/EmptySolidImg/FullSolidImg").gameObject;
        hudCrosshairSprites[0] = transform.Find("HUDCanvas/CrosshairSprites/SolidCrossImg").gameObject;
        hudStateSprites[1] = transform.Find("HUDCanvas/StateSprites/EmptyLiquidImg/FullLiquidImg").gameObject;
        hudCrosshairSprites[1] = transform.Find("HUDCanvas/CrosshairSprites/LiquidCrossImg").gameObject;
        hudStateSprites[2] = transform.Find("HUDCanvas/StateSprites/EmptyGasImg/FullGasImg").gameObject;
        hudCrosshairSprites[2] = transform.Find("HUDCanvas/CrosshairSprites/GasCrossImg").gameObject;
        hudCrosshairSprites[3] = transform.Find("HUDCanvas/CrosshairSprites/DefaultCrossImg").gameObject;

        hudStateSprites[2].SetActive(true);

        if (hudSubtitle == null) 
            Debug.Log("ERROR in HUDController: HUD Subtitle object reference is null.");
        dialogueQ = new();
    }

    private void Update()
    {
        currentDialogue.time -= Time.deltaTime;
        if (currentDialogue.time <= 0)
        {
            hudSubtitle.SetActive(dialogueQ.TryDequeue(out currentDialogue));
            hudSubtitle.transform.Find("Speaker").GetComponent<TextMeshProUGUI>().text = currentDialogue.speaker + ":";
            hudSubtitle.transform.Find("Dialogue").GetComponent<TextMeshProUGUI>().text = currentDialogue.message;
        }
    }

    // Set the progress indicator for the gas mode charge state (0 = 0%, 1 = 100%)
    public void SetGasChargeProgress(float amt)
    {
        amt = Mathf.Clamp01(amt);
        Color c = indicatorLeft.color;
        c = new(Mathf.Lerp(1.0f, 0.25f, amt), c.g, c.b, c.a);

        indicatorLeft.fillAmount = amt / 2.0f; // max at
        indicatorRight.fillAmount = amt / 2.0f;
        indicatorLeft.color = c;
        indicatorRight.color = c;
    }

    // Sets the opacity of the crosshair (between 0 and 1)
    public void SetCrosshairOpacity(float amt)
    {
        foreach (GameObject crosshair in hudCrosshairSprites)
        {
            Color c = crosshair.GetComponent<Image>().color;
            crosshair.GetComponent<Image>().color = new(c.r, c.g, c.b, Mathf.Clamp01(amt));
        }
    }

    // Set the Progess visual of gas mode cooldown (the red crosshair) (between 0 and 1)
    public void SetGasCDProgress(float amt)
    {
        gasCrossRed.fillAmount = Mathf.Clamp01(amt);
    }

    // Set the activated state in the player HUD (setting the active crosshair and bottom-left image).
    public void SetHUDMatterState(MatterState state)
    {
        for (int i = 0; i < 3; i++)
        {
            hudStateSprites[i].SetActive(false);
            hudCrosshairSprites[i].SetActive(false);
        }
        hudCrosshairSprites[3].SetActive(false);

        hudCrosshairSprites[(int)state].SetActive(true);
        hudStateSprites[(int)state].SetActive(true);
    }

    /// <summary>
    /// Add a new subtitle text to the subtitle queue.
    /// </summary>
    /// <param name="speakerName">The name of the one speaking.</param>
    /// <param name="message">The message being spoken.</param>
    /// <param name="time">How long this subtitle should display for.</param>
    public void QueueSubtitle(string speakerName, string message, float time)
    {
        dialogueQ.Enqueue(new Dialogue(speakerName, message, time));
    }

    public void SetVisibility(bool value)
    {
        hudStateSprites[0].transform.parent.parent.gameObject.SetActive(value);
        hudCrosshairSprites[0].transform.parent.gameObject.SetActive(value);
    }
}
