using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Controlador para manejar el menu de pausa en el juego.
/// Permite pausar y reanudar la simulacion, asi como volver al menu principal.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    /// <summary>
    /// GameObject que contiene la interfaz de usuario del menu de pausa.
    /// </summary>
    public GameObject pausePanel;

    /// <summary>
    /// Indica si el juego esta actualmente en estado de pausa.
    /// </summary>
    private bool isPaused = false;

    /// <summary>
    /// Comprueba si se apreto la tecla escape para alternar
    /// entre pausar y reanudar el juego.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    /// <summary>
    /// Activa el menu de pausa y muestra el panel, tambien detiene la simulacion y
    /// desbloquea el cursor para permitir interaccion con la UI.
    /// </summary>
    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    /// <summary>
    /// Reanuda la simulacion desde pausa oculta el panel, restablece el tiempo
    /// y bloquea el cursor para volver al control del jugador.
    /// </summary>
    public void Resume()
    {
        Debug.Log("Test1Resume");
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    /// <summary>
    /// Vuelve al menu principal cargando la escena con indice 0
    /// </summary>
    public void ReturnToMainMenu()
    {
        Debug.Log("Test2MainMenu");
        Time.timeScale = 1f;             // restaura la simu
        SceneManager.LoadScene(0);       // indice 0 del StartMenu
    }
}
