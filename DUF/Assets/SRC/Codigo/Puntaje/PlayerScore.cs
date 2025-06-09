using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Puntuacion del jugador.
/// Tiene una propiedad de solo lectura para acceder a la puntuacion actual
/// y un metodo para poder anadir puntos.
/// </summary>
public class PlayerScore : MonoBehaviour
{
    /// <summary>
    /// Obtiene la puntuacion actual del jugador.
    /// </summary>
    public int currentScore { get; private set; } = 0;

    /// <summary>
    /// Agrega puntos a la puntuacion actual.
    /// </summary>
    public void AddPoints(int points)
    {
        Debug.Log("Suma puntos");
        currentScore += points;
    }
}

