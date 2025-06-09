using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private void Start()
    {
        // inicializamos el tiempo de la carrera
        timeRemaining = raceDuration;

        // ocultamos el panel de resultado al inicio
        resultPanel.SetActive(false);

        // Asegurarnos de que el juego no este en pausa
        Time.timeScale = 1f;
    }

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

    private void UpdateTimerUI()
    {
        // convertimos segundos a mm:ss
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateScoreUI()
    {
        // mostramos puntaje actual de cada jugador
        //player1ScoreText.text = player1Score.currentScore.ToString();
        //player2ScoreText.text = player2Score.currentScore.ToString();
        player1ScoreText.text = $"Jugador 1: {player1Score.currentScore}";
        player2ScoreText.text = $"Jugador 2: {player2Score.currentScore}";
    }

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

    // Llamada de FinishLine para cunado un jugador toca la meta o el objeto de meta
    public void OnPlayerFinish(int playerNumber)
    {
        if (isRaceOver)
            return;

        DeclareWinner(playerNumber);
    }

    private void DeclareWinner(int playerNumber)
    {   
        isRaceOver = true;
        Time.timeScale = 0f;

        resultPanel.SetActive(true);
        resultText.text = $"¡Ganó el Jugador {playerNumber}!";

        resultImage.gameObject.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

    }

    // aun por probar con el puntaje de los dos jugadores REVISAR
    private void DeclareTie()
    {
        isRaceOver = true;
        Time.timeScale = 0f;

        resultPanel.SetActive(true);
        resultText.text = "¡Empate!";
    }

    // revancha
    public void ReplayRace()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    // salir
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); 
    }
}