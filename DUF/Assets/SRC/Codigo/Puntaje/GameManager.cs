using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controlador principal de la partida, ajustar el temporizador, los puntajes de los jugadores,
/// la interfaz de resultados y poder navegar entre escenas como revancha y el menu principal).
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Puntuaciones")]
    public PlayerScore player1Score;  // referencia al PlayerScore de JugadorFP 
    public PlayerScore player2Score;  // referencia al PlayerScore de JugadorFP2

    [Header("UI Elements")]
    public TMP_Text player1ScoreText; // TextMeshPro de puntos de JugadorFP1
    public TMP_Text player2ScoreText; // TextMeshPro de puntos de JugadorFP12
    public TMP_Text timerText;        //TextMeshPro que muestre el temporizador
    public GameObject resultPanel;    // panel de jugador ganador
    public TMP_Text resultText;       // texto dentro de resultPanel que dice "el perico donde quiera es verde"
    public Image resultImage;         // imagen de resultado

    [Header("Configuracion de tiempo")]
    public float raceDuration = 60f;  // 1 minuto
    private float timeRemaining;
    private bool isRaceOver = false;

    /// <summary>
    /// Inicializa el temporizadortambien oculta el panel de resultados y obtiene referencias a los componentes necesarios.
    /// </summary>
    /// <returns>
    /// Coroutine que se espera hasta que existan al menos dos instancias de <see cref="PlayerScore"/> en escena.
    /// </returns>
    private IEnumerator Start()
    {
        // inicializamos el tiempo de la carrera
        timeRemaining = raceDuration;

        // ocultamos el panel de resultado al inicio
        resultPanel.SetActive(false);

        // Asegurarnos de que el juego no este en pausa
        Time.timeScale = 1f;

        
        yield return new WaitUntil(() => FindObjectsOfType<PlayerScore>().Length >= 2);

        var scores = FindObjectsOfType<PlayerScore>();
        player2Score = scores[0];
        player1Score = scores[1];

        // Inicializa UI y tiempo...
        resultPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    /// <summary>
    /// Actualiza el temporizador y los puntajes si la carrera sigue activa.
    /// </summary>
    private void Update()
    {
        if (isRaceOver)
            return;

        // actualizamos temporizador
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndRaceByTime();
        }

        UpdateTimerUI();
        UpdateScoreUI();
    }

    /// <summary>
    /// Actualiza el texto del temporizador.
    /// </summary>
    private void UpdateTimerUI()
    {
        // convertimos segundos a mm:ss
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Actualiza los textos de puntaje de ambos jugadores.
    /// </summary>
    private void UpdateScoreUI()
    {
        // mostramos puntaje actual de cada jugador
        //player1ScoreText.text = player1Score.currentScore.ToString();
        //player2ScoreText.text = player2Score.currentScore.ToString();
        player1ScoreText.text = $"Jugador 1: {player1Score.currentScore}";
        player2ScoreText.text = $"Jugador 2: {player2Score.currentScore}";
    }

    /// <summary>
    /// Determina el ganador al terminar el tiempo comparando los puntajes.
    /// </summary>
    private void EndRaceByTime()
    {
        // determinamos ganador por puntaje
        int p1 = player1Score.currentScore;
        int p2 = player2Score.currentScore;

        if (p1 > p2)

            DeclareWinner(1);
        else if (p2 > p1)
            DeclareWinner(2);
        else
            DeclareTie();

    }

    /// <summary>
    /// Llamado por la linea de meta que es una capsula al tocarla un jugador para declarar ganador de inmediato.
    /// </summary>
    public void OnPlayerFinish(int playerNumber)
    {
        if (isRaceOver)
            return;

        DeclareWinner(playerNumber);
    }

    /// <summary>
    /// Declara ganador y para el juego y muestra el panel de resultado.
    /// </summary>
    private void DeclareWinner(int playerNumber)
    {
        isRaceOver = true;
        Time.timeScale = 0f;

        resultPanel.SetActive(true);
        resultText.text = $"¡Ganó el Jugador {playerNumber}!";

        resultImage.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    /// <summary>
    /// Marca mpate cuando ambos puntajes son iguales.
    /// </summary>
    private void DeclareTie()
    {
        isRaceOver = true;
        Time.timeScale = 0f;

        resultPanel.SetActive(true);
        resultText.text = "¡Empate!";
    }

    /// <summary>
    /// Recarga la escena actual para hace revancha.
    /// </summary>
    public void ReplayRace()
    {
        Time.timeScale = 1f;

        //  escena actualmente activa
        Scene current = SceneManager.GetActiveScene();

        // recargamos por índice 
        SceneManager.LoadScene(current.buildIndex);
    }

    /// <summary>
    /// Sale al menu principal.
    /// </summary>
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    
}