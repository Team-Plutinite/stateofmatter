using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float maxHP;
    public float hp;

    public GameObject checkpointManager;

    private Image hudGlow;

    private bool playerIsDead;

    // Start is called before the first frame update
    void Start()
    {
        playerIsDead = false;

        hp = maxHP;

        hudGlow = this.transform.Find("PlayerHUD").Find("HUDCanvas").Find("Glow").gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsDead)
        {
            hudGlow.color = new Color(hudGlow.color.r, hudGlow.color.g, hudGlow.color.b, 1);
        } else
        {
            hudGlow.color = new Color(hudGlow.color.r, hudGlow.color.g, hudGlow.color.b, 1 - (float)(hp / maxHP));
        }

        if (hp <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        hp = maxHP;
        if (checkpointManager != null)
        {
            StartCoroutine(DeathCycle(2.5f));
        }
    }

    IEnumerator DeathCycle(float wait)
    {
        playerIsDead = true;
        gameObject.GetComponent<PlayerController>().CutsceneMode = true;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f, gameObject.transform.position.z);
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        yield return new WaitForSeconds(wait);

        checkpointManager.GetComponent<CheckpointManager>().RespawnAtCheckpoint();
        playerIsDead = false;
        gameObject.GetComponent<PlayerController>().CutsceneMode = false;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
}
