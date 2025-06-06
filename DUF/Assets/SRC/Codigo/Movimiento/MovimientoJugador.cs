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

    // Start is called before the first frame update
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

    // Update is called once per frame
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

    void FixedUpdate()
    {
        Movimiento();
        //rb.AddForce(9.81f * rb.mass * Vector3.down, ForceMode.Force);

    }

    public void Inputs()
    {
        // inputHorizontal = Input.GetAxisRaw("Horizontal");
        // inputVertical = Input.GetAxisRaw("Vertical");

        input = playerInput.actions["Moverse"].ReadValue<Vector2>();
    }

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

    public void Salto(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed && tocandoPiso)
        {
            puedeSaltar = false;

            saliendoPendiente = true;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * fuarzaSalto, ForceMode.Impulse);
            Invoke(nameof(ResetSalto), coolDownSalto);

        }

    }

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

    private void ResetSalto()
    {
        puedeSaltar = true;

        saliendoPendiente = false;
    }

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
    public void JumpToPosition(Vector3 targetPos, float height)
    {
        activaGancho = true;
        velocidadToSet = CalculateJumpVelocity(transform.position, targetPos, height);
        Invoke(nameof(SetVelocidad), 0.1f);
        Invoke(nameof(ResetRestrictions), 0f);
    }

    private Vector3 velocidadToSet;

    private void SetVelocidad()
    {
        enableMovement = true;
        rb.velocity = velocidadToSet;
    }

    public void ResetRestrictions()
    {
        activaGancho = false;
    }

    private void OllisionEnter(Collision collision)
    {
        if (enableMovement)
        {
            enableMovement = false;
            ResetRestrictions();
            GetComponent<Grappling>().EndGrapple();
        }

    }



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

    public bool EnPendiente()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out tocandoPendiente, alturaJugador * 0.5f + 0.3f))
        {
            float anguloPendiente = Vector3.Angle(Vector3.up, tocandoPendiente.normal);
            return anguloPendiente < maxAunguloPendiente && anguloPendiente != 0f;
        }

        return false;
    }

    public Vector3 GetDireccionMovimientoPendiente(Vector3 direccion)
    {
        return Vector3.ProjectOnPlane(direccion, tocandoPendiente.normal).normalized;
    }
}
