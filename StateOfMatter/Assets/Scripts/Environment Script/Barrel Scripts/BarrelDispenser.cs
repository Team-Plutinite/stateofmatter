using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelDispenser : MonoBehaviour
{
    [Tooltip("Reference to the barrel this dispenser tracks; if this is set, the dispenser will release a new barrel when it is destroyed. " +
        "If this is not set, the dispenser will automatically create and dispense its own barrel at the start.")]
    public GameObject dispensedBarrel;
    public float respawnForce = 750f;

    [Tooltip("Set this to the prefab that will be used to create new barrels with.")]
    [SerializeField] private GameObject barrelPrefab;
    [SerializeField] private Transform spawnPoint;
    

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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == dispensedBarrel)
            other.GetComponent<Rigidbody>().AddForce(transform.forward * respawnForce * Time.deltaTime);
    }

    /// <summary>
    /// Resets the exploded barrel after [delay] seconds, essentially resetting and then redispensing it
    /// </summary>
    private IEnumerator ResetBarrel(float delay)
    {
        // Delay for the specified amount of seconds before resetting
        for (float i = 0.0f; i < delay; i += Time.deltaTime)
            yield return new WaitForSeconds(Time.deltaTime);

        dispensedBarrel.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        dispensedBarrel.GetComponent<Barrel>().Health = 100;
        dispensedBarrel.GetComponent<Barrel>().Exploded = false;
        dispensedBarrel.SetActive(true);
    }
}
