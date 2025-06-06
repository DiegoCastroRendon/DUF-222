using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    // victoria instantanea
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ver puntaje de jugador 1 o el 2
            PlayerScore ps = other.GetComponent<PlayerScore>();
            if (ps != null)
            {
                // linea de meta gana
                if (ps.gameObject.name.Contains("JugadorFP"))
                    gameManager.OnPlayerFinish(1);
                else if (ps.gameObject.name.Contains("JugadorFP2"))
                    gameManager.OnPlayerFinish(2);
            }
        }
    }
}
