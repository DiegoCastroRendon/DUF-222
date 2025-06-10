using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detecta el mmomento en el que un jugador toca la capsula y avisa al GameManager.
/// </summary>
public class FinishLine : MonoBehaviour
{
    /// <summary>
    /// Referencia al controlador de la partida para decir el fin de la carrera.
    /// </summary>
    public GameManager gameManager;

    /// <summary>
    /// Se ejecuta al entrar en el trigger.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ver puntaje de jugador 1 o el 2
            PlayerScore ps = other.GetComponent<PlayerScore>();
            if (ps != null)
            {
                // objeto de meta gana
                if (ps.gameObject.name.Contains("JugadorFP1"))
                    gameManager.OnPlayerFinish(1);
                else if (ps.gameObject.name.Contains("JugadorFP2"))
                    gameManager.OnPlayerFinish(2);
            }
        }
    }
}
