using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject greenOverlay;

    public AudioSource source;
    public AudioClip clickSound;
    public AudioClip backSound;
    public AudioClip hoverSound;

    public bool paused { get; set; }
    void Start()
    {
        paused = false;
        ResumeGame();

        source = gameObject.AddComponent<AudioSource>();
        source.volume = 0.3f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        greenOverlay.SetActive(false);
        paused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        greenOverlay.SetActive(true);
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void GoToMainMenu()
    { 
        paused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(0);
    }

    public void PlayClickSound()
    {
        source.PlayOneShot(clickSound);
    }
    public void PlayBackSound()
    {
        source.PlayOneShot(backSound);
    }

    public void PlayHoverSound()
    {
        source.PlayOneShot(hoverSound);
    }
}
