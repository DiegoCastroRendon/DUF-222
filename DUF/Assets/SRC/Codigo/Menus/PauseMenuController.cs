using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject pausePanel;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void Resume()
    {
        Debug.Log("Test1Resume");
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Test2MainMenu");
        Time.timeScale = 1f;             // restaura la simu
        SceneManager.LoadScene(0);       // indice 0 del StartMenu
    }
}