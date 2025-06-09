using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoCamara : MonoBehaviour
{

    public Transform posicionCamara;

    /// <summary>
    /// Actualiza la posición de la cámara para que coincida con la posición de 'posicionCamara'.
    /// </summary>
    void Update()
    {
        transform.position = posicionCamara.position;
    }
}
