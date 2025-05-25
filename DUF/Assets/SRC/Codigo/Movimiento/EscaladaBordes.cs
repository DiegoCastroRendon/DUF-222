using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("Salto Entre Bordes")]
    public KeyCode salto = KeyCode.Space;
    public float fuerzaSaltoBordeAdelatne;
    public float fuerzaSaltoBordeArriba;

    [Header("Deteccion de Bordes")]
    public float distanciaDeteccionBorde;
    public float radioEsferaCastBorde;
    public LayerMask esBorde;

    private Transform ultimoBorde;
    private Transform bordeActual;

    private RaycastHit bordeHit;

    [Header("Salida")]
    public bool saliendoBorde;
    public float tiempoSalidaBorde;
    public float contadorSaldaBorde;


    void Update()
    {
        DeteccionBordes();
        MaquinaEstado();
    }



    private void DeteccionBordes() {
        bool bordeDetectado = Physics.SphereCast(transform.position, radioEsferaCastBorde, camara.forward, out bordeHit, distanciaDeteccionBorde, esBorde);

        Debug.Log("Borde detectado: " + bordeDetectado);
        {
            
        }
        
        if (!bordeDetectado) return;

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

            if (tiempoEnBorde > tiempoMinEnBorde && cualquirTeclaPrecionada) SalidaSujetandoBorde();

            if (Input.GetKeyDown(salto)) SaltoBorde();
        }
        else if (saliendoBorde)
        {
            if (contadorSaldaBorde > 0) tiempoEnBorde -= Time.deltaTime;
            else saliendoBorde = false;
        }
        
    }

    private void SaltoBorde()
    {
        SalidaSujetandoBorde();

        Invoke(nameof(SaltoBordeDelay), 0.05f);
    }

    private void SaltoBordeDelay()
    {
        Vector3 fuarzaA = camara.forward * fuerzaSaltoBordeAdelatne + orientacion.up * fuerzaSaltoBordeArriba;

        rb.velocity = Vector3.zero;
        rb.AddForce(fuarzaA, ForceMode.Impulse);
    }

    private void EntradaSujetandoBorde()
    {
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
        Vector3 puntoRef = camara.position;

        Vector3 direccionAlBorde = bordeActual.position - orientacion.position;
        float distanciaAlBorde = Vector3.Distance(orientacion.position, bordeActual.position);

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
        saliendoBorde = true;
        contadorSaldaBorde = tiempoSalidaBorde;

        pm.restringido = false;
        pm.congelado = false;
        pm.ilimitado = false;

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

