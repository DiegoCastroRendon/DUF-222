using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{

    [Header("Referencias")]
    private PlayerInput playerInput;

    [Header("Movimiento")]
    private float velocidadMovimiento;
    public float velocidadCaminando;
    public float velocidadCorriendo;
    public float friccionPiso;
    public float velocidadDeslizamiento;
    public float velocidadWallRun;

    private float velocidadMovimientoDeseada;
    private float ultimaVelocidadMovimientoDeseada;

    private Vector2 input;

    [Header("Salto")]
    public float fuarzaSalto;
    public float coolDownSalto;
    public float multiplicadorAire;
    bool puedeSaltar;

    [Header("Agacharse")]
    public float velocidadAgachado;
    public float escalaYAgachado;
    private float inicioEscalY;

    [Header("Controles")]
    public KeyCode teclaSalto = KeyCode.Space;
    public KeyCode teclaCorrer = KeyCode.LeftShift;
    public KeyCode teclaAgacharse = KeyCode.LeftControl;

    [Header("Check piso")]
    public float alturaJugador;
    public LayerMask piso;
    public bool tocandoPiso;

    public Transform orientacion;
    float inputHorizontal;
    float inputVertical;

    [Header("Manejador de pendientes")]
    public float maxAunguloPendiente;
    private RaycastHit tocandoPendiente;
    private bool saliendoPendiente;

    Vector3 direccionMovimiento;

    Rigidbody rb;

    public bool restringido;

    public EstadoMovimiento estadoMovimieto;

    public enum EstadoMovimiento
    {


        caminando,
        corriendo,
        congelado,
        ilimidado,
        wallrunning,
        agachado,
        aire,
        desliz,
    }

    public bool activaGancho;
    public bool deslizandose;
    public bool wallrunning;

    public bool congelado;
    public bool ilimitado;

    /// <summary>
    /// Inicializa las referencias y variables necesarias para el movimiento del jugador.
    /// </summary>
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;
        rb.mass = 1f;

        puedeSaltar = true;

        inicioEscalY = transform.localScale.y;
    }

    /// <summary>
    /// Actualiza el estado del jugador, gestiona entradas y controla la velocidad.
    /// </summary>
    void Update()
    {

        tocandoPiso = Physics.Raycast(transform.position, Vector3.down, alturaJugador * 0.5f + 0.2f, piso);

        Debug.DrawRay(transform.position, Vector3.down * (alturaJugador * 0.5f + 0.2f), Color.red);

        Inputs();
        ControlVelocidad();
        ManejadorEstadoMovimiento();

        if (tocandoPiso && !activaGancho)
        {
            rb.drag = friccionPiso;
        }
        else
        {
            rb.drag = 0f;
        }

    }

    /// <summary>
    /// Aplica la física de movimiento del jugador.
    /// </summary>
    void FixedUpdate()
    {
        Movimiento();
        //rb.AddForce(9.81f * rb.mass * Vector3.down, ForceMode.Force);

    }

    /// <summary>
    /// Lee la entrada del jugador para el movimiento.
    /// </summary>
    public void Inputs()
    {
        // inputHorizontal = Input.GetAxisRaw("Horizontal");
        // inputVertical = Input.GetAxisRaw("Vertical");

        input = playerInput.actions["Moverse"].ReadValue<Vector2>();
    }

    /// <summary>
    /// Gestiona el movimiento del jugador, incluyendo pendientes y diferentes estados.
    /// </summary>
    private void Movimiento()
    {

        if (activaGancho) return;

        if (restringido) return;

        direccionMovimiento = (orientacion.forward * input.y) + (orientacion.right * input.x);

        if (EnPendiente() && !saliendoPendiente)
        {
            rb.AddForce(20f * velocidadMovimiento * GetDireccionMovimientoPendiente(direccionMovimiento), ForceMode.Force);

            if (rb.velocity.y > 0f)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }


        if (tocandoPiso)
            rb.AddForce(10f * velocidadMovimiento * direccionMovimiento.normalized, ForceMode.Force);

        else if (!tocandoPiso)
            rb.AddForce(10f * multiplicadorAire * velocidadMovimiento * direccionMovimiento.normalized, ForceMode.Force);

        if (wallrunning) rb.useGravity = !EnPendiente();


    }

    /// <summary>
    /// Controla la velocidad máxima del jugador según el estado y el entorno.
    /// </summary>
    private void ControlVelocidad()
    {
        if (activaGancho) return;

        if (EnPendiente() && !saliendoPendiente)
        {
            if (rb.velocity.magnitude > velocidadMovimiento)
            {
                rb.velocity = rb.velocity.normalized * velocidadMovimiento;
            }
        }
        else
        {
            Vector3 velPlana = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (velPlana.magnitude > velocidadMovimiento)
            {
                Vector3 velLimite = velPlana.normalized * velocidadMovimiento;
                rb.velocity = new Vector3(velLimite.x, rb.velocity.y, velLimite.z);
            }
        }
    }

    /// <summary>
    /// Ejecuta el salto si se cumplen las condiciones.
    /// </summary>
    /// <param name="callbackContext">Contexto de la acción de entrada.</param>
    public void Salto(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed && tocandoPiso)
        {
            Debug.Log("Boton salto presionado");
            puedeSaltar = false;

            saliendoPendiente = true;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * fuarzaSalto, ForceMode.Impulse);
            Invoke(nameof(ResetSalto), coolDownSalto);

        }

    }

    /// <summary>
    /// Gestiona el estado de agacharse del jugador.
    /// </summary>
    public void Agacharce(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started || callbackContext.performed)
        {
            transform.localScale = new Vector3(transform.localScale.x, escalaYAgachado, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (callbackContext.canceled)
        {
            transform.localScale = new Vector3(transform.localScale.x, inicioEscalY, transform.localScale.z);
        }
    }

    /// <summary>
    /// Gestiona el estado de correr del jugador.
    /// </summary>
    /// <param name="callbackContext">Contexto de la acción de entrada.</param>
    public void Correr(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed && tocandoPiso)
        {
            Debug.Log("Corriendo");
            velocidadMovimientoDeseada = velocidadCorriendo;
        }

        if (callbackContext.canceled)
        {
            velocidadMovimientoDeseada = velocidadCaminando;
        }
    }

    /// <summary>
    /// Restablece la capacidad de saltar y el estado de pendiente.
    /// </summary>
    private void ResetSalto()
    {
        puedeSaltar = true;

        saliendoPendiente = false;
    }

    /// <summary>
    /// Calcula la velocidad necesaria para saltar de un punto a otro con una altura específica.
    /// </summary>
    /// <param name="startPoint">Punto de inicio.</param>
    /// <param name="endPoint">Punto de destino.</param>
    /// <param name="trajectoryHeight">Altura máxima de la trayectoria.</param>
    /// <returns>Vector de velocidad calculado.</returns>
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ /
            (Mathf.Sqrt(-2 * trajectoryHeight / gravity) +
            Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return (velocityXZ * 2) + (velocityY * 1.5f);
    }


    private bool enableMovement;
    /// <summary>
    /// Realiza un salto hacia una posición objetivo con una altura determinada.
    /// </summary>
    /// <param name="targetPos">Posición objetivo.</param>
    /// <param name="height">Altura del salto.</param>
    public void JumpToPosition(Vector3 targetPos, float height)
    {
        activaGancho = true;
        velocidadToSet = CalculateJumpVelocity(transform.position, targetPos, height);
        Invoke(nameof(SetVelocidad), 0.1f);
        Invoke(nameof(ResetRestrictions), 0f);
    }

    private Vector3 velocidadToSet;

    /// <summary>
    /// Asigna la velocidad calculada al Rigidbody.
    /// </summary>
    private void SetVelocidad()
    {
        enableMovement = true;
        rb.velocity = velocidadToSet;
    }

    /// <summary>
    /// Restablece las restricciones de movimiento después de usar el gancho.
    /// </summary>
    public void ResetRestrictions()
    {
        activaGancho = false;
    }

    /// <summary>
    /// Maneja la colisión al aterrizar después de usar el gancho.
    /// </summary>
    /// <param name="collision">Información de la colisión.</param>    
    private void OllisionEnter(Collision collision)
    {
        if (enableMovement)
        {
            enableMovement = false;
            ResetRestrictions();
            GetComponent<Grappling>().EndGrapple();
        }

    }


    /// <summary>
    /// Gestiona el estado de movimiento del jugador según las condiciones actuales.
    /// </summary>
    private void ManejadorEstadoMovimiento()
    {


        if (congelado)
        {
            estadoMovimieto = EstadoMovimiento.congelado;
            velocidadMovimiento = 0;
            rb.velocity = Vector3.zero;
        }

        else if (ilimitado)
        {
            estadoMovimieto = EstadoMovimiento.ilimidado;
            velocidadMovimiento = 999f;
            return;
        }

        else if (wallrunning)
        {
            estadoMovimieto = EstadoMovimiento.wallrunning;
            velocidadMovimientoDeseada = velocidadWallRun;
        }
        else if (deslizandose && playerInput.actions["Deslizarse"].IsPressed())
        {
            estadoMovimieto = EstadoMovimiento.desliz;
            if (EnPendiente() && rb.velocity.y > 0f)
            {
                velocidadMovimientoDeseada = velocidadDeslizamiento;
            }
            else
            {
                velocidadMovimientoDeseada = velocidadCorriendo;
            }
        }

        else if (playerInput.actions["Agacharse"].IsPressed())
        {
            estadoMovimieto = EstadoMovimiento.agachado;
            velocidadMovimientoDeseada = velocidadAgachado;
        }

        else if (tocandoPiso && playerInput.actions["Correr"].IsPressed())
        {
            estadoMovimieto = EstadoMovimiento.corriendo;
            velocidadMovimientoDeseada = velocidadCorriendo;
        }

        else if (tocandoPiso)
        {
            estadoMovimieto = EstadoMovimiento.caminando;
            velocidadMovimientoDeseada = velocidadCaminando;
        }

        else
        {
            estadoMovimieto = EstadoMovimiento.aire;
        }

        if (Mathf.Abs(velocidadMovimientoDeseada - ultimaVelocidadMovimientoDeseada) > 4f && velocidadMovimiento != 0)
        {
            StopAllCoroutines();
            StartCoroutine(PerdidadVelocidadMovimietnoSueave());
        }
        else
        {
            velocidadMovimiento = velocidadMovimientoDeseada;
        }

        ultimaVelocidadMovimientoDeseada = velocidadMovimientoDeseada;

    }

    /// <summary>
    /// Corrutina para suavizar la transición de velocidad al cambiar de estado.
    /// </summary>
    /// <returns>IEnumerator para la corrutina.</returns>
    private IEnumerator PerdidadVelocidadMovimietnoSueave()
    {
        float tiempo = 0f;
        float diferencia = Mathf.Abs(velocidadMovimientoDeseada - velocidadMovimiento);
        float valorInical = velocidadMovimiento;

        while (tiempo < diferencia)
        {
            velocidadMovimiento = Mathf.Lerp(valorInical, velocidadMovimientoDeseada, tiempo / diferencia);
            yield return null;
        }

        velocidadMovimiento = velocidadMovimientoDeseada;
    }

    /// <summary>
    /// Determina si el jugador está sobre una pendiente válida.
    /// </summary>
    /// <returns>True si está en pendiente, de lo contrario false.</returns>
    public bool EnPendiente()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out tocandoPendiente, alturaJugador * 0.5f + 0.3f))
        {
            float anguloPendiente = Vector3.Angle(Vector3.up, tocandoPendiente.normal);
            return anguloPendiente < maxAunguloPendiente && anguloPendiente != 0f;
        }

        return false;
    }

    /// <summary>
    /// Calcula la dirección de movimiento ajustada a la pendiente.
    /// </summary>
    /// <param name="direccion">Dirección original de movimiento.</param>
    /// <returns>Dirección ajustada a la pendiente.</returns>
    public Vector3 GetDireccionMovimientoPendiente(Vector3 direccion)
    {
        return Vector3.ProjectOnPlane(direccion, tocandoPendiente.normal).normalized;
    }
}
