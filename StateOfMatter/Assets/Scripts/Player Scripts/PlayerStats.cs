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


    public AudioSource source;
    public AudioClip slowHeartbeatSound;
    public AudioClip fastHeartbeatSound;

    private float heartbeatSoundTimer;
    private const float heartbeatSoundCooldown = 1.57f;

    void Start()
    {
        playerIsDead = false;

        hp = maxHP;

        hudGlow = this.transform.Find("PlayerHUD").Find("HUDCanvas").Find("Glow").gameObject.GetComponent<Image>();

        heartbeatSoundTimer = heartbeatSoundCooldown;

    }

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

        if (hp > 1 && hp < 5 && heartbeatSoundTimer<=0.0f)
        {
            source.PlayOneShot(slowHeartbeatSound);
            heartbeatSoundTimer = heartbeatSoundCooldown;
        }
        if (hp == 1 && heartbeatSoundTimer <= 0.0f)
        {
            source.PlayOneShot(fastHeartbeatSound);
            heartbeatSoundTimer = heartbeatSoundCooldown;
        }

        heartbeatSoundTimer -= Time.deltaTime;
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
