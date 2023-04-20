using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    // Gas mode sprites
    private Image indicatorLeft;
    private Image indicatorRight;
    private Image gasCrossRed;

    private GameObject[] hudStateSprites;
    private GameObject[] hudCrosshairSprites;

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
}
