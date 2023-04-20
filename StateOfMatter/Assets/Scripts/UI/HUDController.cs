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
        indicatorLeft = transform.Find("HUDCanvas/CrosshairSprites/GasCrossImg/ChargeUp_Indicator_Left").GetComponent<Image>();
        indicatorRight = transform.Find("HUDCanvas/CrosshairSprites/GasCrossImg/ChargeUp_Indicator_Right").GetComponent<Image>();
        gasCrossRed = transform.Find("HUDCanvas/CrosshairSprites/GasCrossImg/GasCrossImgRed").GetComponent<Image>();
        if (indicatorLeft == null) Debug.Log("ERROR in Charge-Up Controller: left indicator image was not found.");
        if (indicatorRight == null) Debug.Log("ERROR in Charge-Up Controller: right indicator image was not found.");

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
        indicatorLeft.fillAmount = amt / 2.0f; // max at
        indicatorRight.fillAmount = amt / 2.0f;
    }

    public void SetCrosshairOpacity(float amt)
    {
        foreach (GameObject crosshair in hudCrosshairSprites)
        {
            Color c = crosshair.GetComponent<Image>().color;
            crosshair.GetComponent<Image>().color = new(c.r, c.g, c.b, Mathf.Clamp01(amt));
        }
    }

    public void SetGasCDProgress(float amt)
    {
        gasCrossRed.fillAmount = Mathf.Clamp01(amt);
    }

    // Set the activate state in the player HUD.
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
