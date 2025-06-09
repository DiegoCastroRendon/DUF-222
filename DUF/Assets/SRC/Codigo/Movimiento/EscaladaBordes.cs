using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class EscaladaBordes : MonoBehaviour
{
    [Header("Referencias")]
    public MovimientoJugador pm;
    public Transform orientacion;
    public Transform camara;
    public Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 input;

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

    /// <summary>
    /// Inicializa la referencia al componente PlayerInput.
    /// </summary>
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    /// <summary>
    /// Llama a la detección de bordes y a la máquina de estados en cada frame.
    /// </summary>
    void Update()
    {
        DeteccionBordes();
        MaquinaEstado();
    }


    /// <summary>
    /// Detecta si el jugador está cerca de un borde y gestiona la lógica para sujetarse.
    /// </summary>
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

    /// <summary>
    /// Controla el estado del jugador respecto a los bordes (sujetando, saliendo, etc).
    /// </summary>
    private void MaquinaEstado()
    {
        input = playerInput.actions["Moverse"].ReadValue<Vector2>();

        float inputHorizontal = input.y;
        float inputVertical = input.x;

        bool cualquirTeclaPrecionada = inputHorizontal != 0 || inputVertical != 0;

        if (sujetando)
        {
            CongelarRBEnBorde();

            tiempoEnBorde += Time.deltaTime;

            if (tiempoEnBorde > tiempoMinEnBorde && cualquirTeclaPrecionada) SalidaSujetandoBorde();

            if (playerInput.actions["Saltar"].IsPressed()) SaltoBorde();
        }
        else if (saliendoBorde)
        {
            if (contadorSaldaBorde > 0) tiempoEnBorde -= Time.deltaTime;
            else saliendoBorde = false;
        }

    }

    /// <summary>
    /// Realiza el salto desde el borde.
    /// </summary>
    public void SaltoBorde()
    {
        SalidaSujetandoBorde();

        Invoke(nameof(SaltoBordeDelay), 0.05f);
    }

    /// <summary>
    /// Aplica la fuerza de salto después de un pequeño retardo.
    /// </summary>
    private void SaltoBordeDelay()
    {
        Vector3 fuarzaA = camara.forward * fuerzaSaltoBordeAdelatne + orientacion.up * fuerzaSaltoBordeArriba;

        rb.velocity = Vector3.zero;
        rb.AddForce(fuarzaA, ForceMode.Impulse);
    }

    /// <summary>
    /// Activa el estado de sujetar el borde y ajusta las restricciones del jugador.
    /// </summary>
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
    
    /// <summary>
    /// Congela el Rigidbody y mueve al jugador hacia el borde mientras lo está sujetando.
    /// </summary>
    private void CongelarRBEnBorde()
    {
        rb.useGravity = false;
        Vector3 puntoRef = camara.position;

        Vector3 direccionAlBorde = bordeActual.position - orientacion.position;
        float distanciaAlBorde = Vector3.Distance(orientacion.position, bordeActual.position);

        if (distanciaAlBorde > 1f)
        {

            if (rb.velocity.magnitude < velocidadMovABorde)
            {
                rb.AddForce(1000f * Time.deltaTime * velocidadMovABorde * direccionAlBorde.normalized);
            }


        }
        else
        {
            if (!pm.congelado) pm.congelado = true;
            if (pm.ilimitado) pm.ilimitado = false;
        }

        if (distanciaAlBorde > distanciaMaxSujetarBorde) SalidaSujetandoBorde();
    }
    
    /// <summary>
    /// Sale del estado de sujetar el borde y restablece las restricciones.
    /// </summary>
    private void SalidaSujetandoBorde()
    {
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

    /// <summary>
    /// Reinicia la referencia al último borde para permitir volver a sujetarse.
    /// </summary>
    private void ReiniciarUltimoBorde()
    {
        ultimoBorde = null;
    }
}

