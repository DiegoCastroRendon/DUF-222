using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartMenuController : MonoBehaviour
{
    public void TestRoom()
    {
        // Carga la escena 1
        SceneManager.LoadScene(1);
    }

    public void PlayGame()
    {
        // Carga la escena 2
        SceneManager.LoadScene(4);
    }
    
     public void mapOne()
    {
      
        SceneManager.LoadScene(2);
    }
    public void mapTwo()
    {
        SceneManager.LoadScene(3);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
