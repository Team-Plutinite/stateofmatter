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
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Resets the exploded barrel, essentially resetting and then redispensing it
    /// </summary>
    public void DispenseNewBarrel()
    {
        dispensedBarrel.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
    }
}
