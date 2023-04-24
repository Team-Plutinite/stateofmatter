using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controlsMenu;
    public GameObject optionsMenu;

    public AudioSource source;
    public AudioClip clickSound;
    public AudioClip backSound;
    public AudioClip hoverSound;
    public AudioClip playSound;

    public void Start()
    {
        source.volume = 0.3f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsMenu.activeInHierarchy)
                BackButtonPressed();
            else if (controlsMenu.activeInHierarchy)
                BackButtonPressed();
            else
                QuitGame();
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void BackButtonPressed()
    {
        mainMenu.SetActive(true);
        controlsMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ControlsMenu()
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);

    }
    public void OptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
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

    public void PlayPlaySound()
    {
        source.PlayOneShot(playSound);
    }
}
