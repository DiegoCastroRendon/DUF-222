using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Esclar : MonoBehaviour
{
    [Header("Refencias")]

    public Transform orientacion;
    public Rigidbody rb;
    public EscaladaBordes eb;
    public MovimientoJugador pm;
    public LayerMask esPared;

    private PlayerInput playerInput;

    [Header("Escalada")]
    public float velocidadEscalada;
    public float tiempoEscaladaMax;
    private float contadorEscalada;

    public bool escalando;


    [Header("Deteccion")]
    public float distanciaDeteccion;
    public float radioEsferaCompartido;
    public float anguloMaximoVisionPared;
    private float anguloVisionPared;

    private RaycastHit paredFrontalHit;
    public bool paredFrente;

    /// <summary>
    /// Inicializa las referencias necesarias para la escalada.
    /// </summary>
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        eb = GetComponent<EscaladaBordes>();
    }

    /// <summary>
    /// Llama a la comprobación de pared, la máquina de estados y gestiona el movimiento de escalada.
    /// </summary>
    void Update()
    {
        ParedCheck();
        MaquinaEstado();

        if (escalando)
        {
            MoviminetoEscalada();
        }
    }

    /// <summary>
    /// Controla el estado de la escalada según las condiciones del entorno y la entrada del jugador.
    /// </summary>
    private void MaquinaEstado()
    {
        if (eb.sujetando)
        {
            if (escalando) PararEscalada();
        }

        else if (paredFrente && playerInput.actions["Moverse"].IsPressed() && anguloVisionPared < anguloMaximoVisionPared)
        {
            if (!escalando && contadorEscalada > 0) IniciarEscalada();

            if (contadorEscalada > 0) contadorEscalada -= Time.deltaTime;
            if (contadorEscalada < 0) PararEscalada();
        }
        else
        {
            if (escalando) PararEscalada();
        }
    }

    /// <summary>
    /// Comprueba si hay una pared frente al jugador y calcula el ángulo de visión.
    /// </summary>
    private void ParedCheck() {
        paredFrente = Physics.SphereCast(transform.position, radioEsferaCompartido, orientacion.forward, out paredFrontalHit, distanciaDeteccion, esPared);
        anguloVisionPared = Vector3.Angle(orientacion.forward, -paredFrontalHit.normal);

        if (pm.tocandoPiso)
        {
            contadorEscalada = tiempoEscaladaMax;
        }
    }
    
    /// <summary>
    /// Inicia el estado de escalada.
    /// </summary>
    private void IniciarEscalada() {
        escalando = true;
    }
    
    /// <summary>
    /// Aplica el movimiento vertical durante la escalada.
    /// </summary>
    private void MoviminetoEscalada()
    {
        rb.velocity = new Vector3(rb.velocity.x, velocidadEscalada, rb.velocity.z);
    }
    
    /// <summary>
    /// Detiene el estado de escalada.
    /// </summary>
    private void PararEscalada()
    {
        escalando = false;
    }
}
