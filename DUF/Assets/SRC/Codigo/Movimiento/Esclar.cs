using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class Esclar : MonoBehaviour
{
    [Header("Refencias")]

    public Transform orientacion;
    public Rigidbody rb;
    public MovimientoJugador pm;
    public LayerMask esPared;

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
        if (paredFrente && Input.GetKey(KeyCode.W) && anguloVisionPared < anguloMaximoVisionPared)
        {
            if (!escalando && contadorEscalada > 0) IniciarEscalada();

            if(contadorEscalada > 0) contadorEscalada -= Time.deltaTime;
            if(contadorEscalada < 0) PararEscalada();
        } else {
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
