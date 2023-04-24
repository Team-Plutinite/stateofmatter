using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelDispenser : MonoBehaviour
{
    [Tooltip("Reference to the barrel this dispenser tracks; if this is set, the dispenser will release a new barrel when it is destroyed. " +
        "If this is not set, the dispenser will automatically create and dispense its own barrel at the start.")]
    public GameObject dispensedBarrel;

    [Tooltip("Set this to the prefab that will be used to create new barrels with.")]
    public GameObject barrelPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (dispensedBarrel == null)
        {
            try
            {
                dispensedBarrel = Instantiate(barrelPrefab, transform.position, Quaternion.identity);
            }
            catch
            {
                if (barrelPrefab == null) Debug.Log($"({name}) -> ERROR creating barrel: barrelPrefab is null. You probably didn't set it in the inspector.");
            }
        }
        dispensedBarrel.GetComponent<Barrel>().OnDestroyed += () => StartCoroutine(ResetBarrel(1f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Resets the exploded barrel after [delay] seconds, essentially resetting and then redispensing it
    /// </summary>
    private IEnumerator ResetBarrel(float delay)
    {
        // Delay for the specified amount of seconds before resetting
        for (float i = 0.0f; i < delay; i += Time.deltaTime)
            yield return new WaitForSeconds(Time.deltaTime);

        dispensedBarrel.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        dispensedBarrel.GetComponent<Barrel>().health = 100;
        dispensedBarrel.GetComponent<Barrel>().exploded = false;
        dispensedBarrel.SetActive(true);
    }
}
