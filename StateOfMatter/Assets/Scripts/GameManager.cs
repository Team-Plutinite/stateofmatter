using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject greenOverlay;

    public AudioSource source;
    public AudioClip clickSound;
    public AudioClip backSound;
    public AudioClip hoverSound;

    public GameObject pauseUI;
    public GameObject optionsUI;

    public AudioMixer volumeMixer;

    public bool paused { get; set; }
    void Start()
    {
        paused = false;
        ResumeGame();

        source.volume = 0.3f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused && pauseUI.activeInHierarchy)
                ResumeGame();
            else if (paused && optionsUI.activeInHierarchy)
            {
                pauseUI.SetActive(true);
                optionsUI.SetActive(false);
            }
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        greenOverlay.SetActive(false);
        paused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public void PauseGame()
    {
        Time.timeScale = 0;
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

    public void QuitGame()
    {
        Application.Quit();
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

    // ui sliders
    public void SetSFXVolume(float volume)
    {

        volumeMixer.SetFloat("sfxVolume", Mathf.Log10(volume)*20);
    }

    public void SetMusicVolume(float volume)
    {
        volumeMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
        //Debug.Log(volume);
    }
}
