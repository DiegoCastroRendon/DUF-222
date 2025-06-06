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


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        eb = GetComponent<EscaladaBordes>();
    }

    void Update()
    {
        ParedCheck();
        MaquinaEstado();

        if (escalando)
        {
            MoviminetoEscalada();
        }
    }

    private void MaquinaEstado() {
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


    private void ParedCheck() {
        paredFrente = Physics.SphereCast(transform.position, radioEsferaCompartido, orientacion.forward, out paredFrontalHit, distanciaDeteccion, esPared);
        anguloVisionPared = Vector3.Angle(orientacion.forward, -paredFrontalHit.normal);

        if (pm.tocandoPiso)
        {
            contadorEscalada = tiempoEscaladaMax;
        }
    }
    

    private void IniciarEscalada() {
        escalando = true;
    }
    private void MoviminetoEscalada() {
        rb.velocity = new Vector3(rb.velocity.x, velocidadEscalada, rb.velocity.z);
    }
    private void PararEscalada() {
        escalando = false;
    }
}
