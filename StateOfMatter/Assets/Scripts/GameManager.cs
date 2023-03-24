using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject greenOverlay;

    public bool paused { get; set; }
    void Start()
    {
        paused = false;
        ResumeGame();
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
}
