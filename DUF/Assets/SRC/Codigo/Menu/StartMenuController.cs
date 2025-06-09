using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Controlador deel menu de inicio del juego.
/// Proporciona metodos para poder navegar entre escenas como mapas, reglas y test
/// y para salir de la aplicación.
/// </summary>

public class StartMenuController : MonoBehaviour
{
    /// <summary>
    /// Carga la escena TestRoom.
    /// </summary>
    public void TestRoom()
    {
        // Carga la escena 1
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Comienza la partida principal cargando la escena de juego seleccionando el mapa para jugar.
    /// </summary>
    public void PlayGame()
    {
        // Carga la escena 2
        SceneManager.LoadScene(4);
    }
    
    /// <summary>
    /// Carga el Mapa 1.
    /// </summary>
     public void mapOne()
    {
      
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Carga el Mapa 2.
    /// </summary>
    public void mapTwo()
    {
        SceneManager.LoadScene(3);
    }

    /// <summary>
    /// Carga la escena de reglas.
    /// </summary>
    public void rules()
    {
        SceneManager.LoadScene(5);
    }

    /// <summary>
    /// Sale del juego. 
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
