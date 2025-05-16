using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscaladaBordes : MonoBehaviour
{
    [Header("Referencias")]
    public MovimientoJugador pm;
    public Transform orientacion;
    public Transform camara;
    public Rigidbody rb;

    [Header("Sujetar Bordes")]
    public float velocidadMovABorde;
    public float distanciaMaxSujetarBorde;

    public float tiempoMinEnBorde;
    private float tiempoEnBorde;

    public bool sujetando;

    [Header("Deteccion de Bordes")]
    public float distanciaDeteccionBorde;
    public float radioEsferaCastBorde;
    public LayerMask esBorde;

    private Transform ultimoBorde;
    private Transform bordeActual;

    private RaycastHit bordeHit;


    void Update()
    {
        DeteccionBordes();
        MaquinaEstado();
    }



    private void DeteccionBordes() {
        bool bordeDetectado = Physics.SphereCast(transform.position, radioEsferaCastBorde, camara.forward, out bordeHit, distanciaDeteccionBorde, esBorde);

        if(!bordeDetectado) return;

        float distalciaAlBorde = Vector3.Distance(transform.position, bordeHit.transform.position);

        if (bordeHit.transform == ultimoBorde) return;

        if (distalciaAlBorde < distanciaMaxSujetarBorde && !sujetando) EntradaSujetandoBorde();


    }

    private void MaquinaEstado() {
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");
        bool cualquirTeclaPrecionada = inputHorizontal != 0 || inputVertical != 0;

        if (sujetando)
        {
            CongelarRBEnBorde();

            tiempoEnBorde += Time.deltaTime;

            if(tiempoEnBorde > tiempoMinEnBorde && cualquirTeclaPrecionada) SalidaSujetandoBorde();
        }
        
    }

    private void EntradaSujetandoBorde() {
        sujetando = true;

        pm.ilimitado = true;
        pm.restringido = true;

        bordeActual = bordeHit.transform;
        ultimoBorde = bordeHit.transform;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }
    private void CongelarRBEnBorde() {
        rb.useGravity = false;

        Vector3 direccionAlBorde = bordeActual.position - transform.position;
        float distanciaAlBorde = Vector3.Distance(transform.position, bordeActual.position);

        if(distanciaAlBorde > 1f) {

            if(rb.velocity.magnitude < velocidadMovABorde) {
                rb.AddForce(1000f * Time.deltaTime * velocidadMovABorde * direccionAlBorde.normalized);
            }


        } else {
            if (!pm.congelado) pm.congelado = true;
            if(pm.ilimitado) pm.ilimitado = false;
        }

        if(distanciaAlBorde > distanciaMaxSujetarBorde) SalidaSujetandoBorde();
    }
    private void SalidaSujetandoBorde() {
        pm.restringido = false;
        pm.congelado = false;

        sujetando = false;

        tiempoEnBorde = 0f;

        rb.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(ReiniciarUltimoBorde), 1f);
    }

    private void ReiniciarUltimoBorde() {
        ultimoBorde = null;
    }
}

