using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    private Image indicator;
    private GameObject[] hudStateSprites;
    private GameObject[] hudCrosshairSprites;

    // Start is called before the first frame update
    void Start()
    {
        indicator = transform.Find("HUDCanvas/CrosshairSprites/GasCrossImg/ChargeUp_Indicator").GetComponent<Image>();
        if (indicator == null) Debug.Log("ERROR in Charge-Up Controller: indicator image was not found.");

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
    public void SetProgress(float amt)
    {
        indicator.fillAmount = amt;
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
