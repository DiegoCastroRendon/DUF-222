using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    //  puntos da esta bolita al recogerse
    public int pointValue = 1;

    //momento de tocar o atravesar
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScore ps = other.GetComponent<PlayerScore>();
            if (ps != null)
            {
                ps.AddPoints(pointValue);
            }

            // destruimos la bolita
            Destroy(gameObject);
        }
    }
}

