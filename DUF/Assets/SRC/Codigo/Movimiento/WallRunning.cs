using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Controla la mecánica de wallrunning (correr en paredes) del jugador.
/// </summary>
public class WallRunning : MonoBehaviour
{

    [Header("Wallrunning")]
    public LayerMask esPared;
    public LayerMask esSuelo;
    public float fuerzaWallRun;
    public float fuerzaUpWallJump;
    public float fuerzaSideWallJump;

    public float velocidadSubirPared;
    public float tiempoMaximoWallRun;
    private float contadorWallRun;
    [Header("Controles")]
    public KeyCode taclaJump = KeyCode.Space;
    public KeyCode WallRunArriba = KeyCode.LeftShift;
    public KeyCode WallRunAbajo = KeyCode.LeftControl;
    private bool runArriba;
    private bool runAbajo;
    private float horizontalInput;
    private float verticalInput;
    [Header("Deteccion")]
    public float checaDistanciaPared;
    public float minAlturaSalto;
    private RaycastHit izqWallHit;
    private RaycastHit derWallHit;
    private bool paredIzq;
    private bool paredDer;

    [Header("Saliendo")]
    private bool saliendoPared;
    public float tiempoSalidaPared;
    private float contadorTiempoSalida;

    [Header("Gravedad")]
    public bool useGravity;
    public float antiGravedad;


    [Header("Referencias")]
    public Transform orientacion;
    public CamaraFP cam;
    private MovimientoJugador pm;
    private Rigidbody rb;
    private EscaladaBordes eb;
    private PlayerInput playerInput;
    Vector2 input;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<MovimientoJugador>();
        eb = GetComponent<EscaladaBordes>();
    }

    // Update is called once per frame
    void Update()
    {
        VerificaPared();
        Estados();
    }

    void FixedUpdate()
    {
        if (pm.wallrunning)
        {
            WallRunMov();
        }
    }

    /// <summary>
    /// Verifica si hay paredes a izquierda o derecha.
    /// </summary>
    private void VerificaPared()
    {
        paredDer = Physics.Raycast(transform.position, orientacion.right, out derWallHit, checaDistanciaPared, esPared);
        paredIzq = Physics.Raycast(transform.position, -orientacion.right, out izqWallHit, checaDistanciaPared, esPared);

    }

    /// <summary>
    /// Verifica si el jugador está lo suficientemente lejos del suelo para wallrun.
    /// </summary>
    private bool SobreSuelo()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minAlturaSalto, esSuelo);
    }

    /// <summary>
    /// Gestiona los estados de entrada y wallrun.
    /// </summary>
    private void Estados()
    {
        input = playerInput.actions["Moverse"].ReadValue<Vector2>();
        //Controles
        horizontalInput = input.y;
        verticalInput = input.x;

        //runArriba = Input.GetKey(WallRunArriba);
        //runAbajo = Input.GetKey(WallRunAbajo);

        // Estado de wallrun
        if ((paredIzq || paredDer) && verticalInput > 0 && SobreSuelo() && !saliendoPared)
        {
            //Empezar wallrun
            if (!pm.wallrunning)
            {
                StartWallRun();
            }

            if (contadorWallRun > 0)
            {
                contadorWallRun -= Time.deltaTime;
            }

            if (contadorWallRun <= 0 && pm.wallrunning)
            {
                saliendoPared = true;
                contadorTiempoSalida = tiempoSalidaPared;
            }

            if (playerInput.actions["Saltar"].IsPressed())
            {
                WallJump();
            }

        }
        else if (saliendoPared)
        {
            if (pm.wallrunning)
            {
                StopWallRun();
            }

            if (contadorTiempoSalida > 0)
            {
                contadorTiempoSalida -= Time.deltaTime;
            }
            if (contadorTiempoSalida <= 0)
            {
                saliendoPared = false;
            }
        }
        else
        {

            if (pm.wallrunning)
            {
                StopWallRun();
            }
        }
    }

    /// <summary>
    /// Inicia el wallrun.
    /// </summary>
    private void StartWallRun()
    {
        pm.wallrunning = true;

        contadorWallRun = tiempoMaximoWallRun;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //efectos de la camara
        cam.DoFoV(90f);
        if (paredIzq) cam.DoTilt(-5f);
        if (paredDer) cam.DoTilt(5f);

    }


    /// <summary>
    /// Aplica movimiento durante el wallrun.
    /// </summary>
    private void WallRunMov()
    {

        rb.useGravity = useGravity;


        Vector3 wallNormal = paredDer ? derWallHit.normal : izqWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);



        if ((orientacion.forward - wallForward).magnitude > (orientacion.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * fuerzaWallRun, ForceMode.Force);

        if (playerInput.actions["Correr"].IsPressed())
        {
            rb.velocity = new Vector3(rb.velocity.x, velocidadSubirPared, rb.velocity.z);
        }
        if (playerInput.actions["Agacharse"].IsPressed())
        {
            rb.velocity = new Vector3(rb.velocity.x, -velocidadSubirPared, rb.velocity.z);
        }

        //Empujar a la pared
        if (!(paredIzq && horizontalInput > 0) && !(paredDer && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        // Debilitar un poco la gravedad 
        if (useGravity)
        {
            rb.AddForce(transform.up * antiGravedad, ForceMode.Force);
        }



    }


    /// <summary>
    /// Detiene el wallrun y reinicia los efectos visuales.
    /// </summary>
    private void StopWallRun()
    {
        pm.wallrunning = false;

        // quitar efectos de la camara
        cam.DoFoV(80f);
        cam.DoTilt(0f);

    }

    /// <summary>
    /// Funcion para ejecutar un salto desde la pared.
    /// </summary
    private void WallJump()
    {
        if (eb.sujetando || eb.saliendoBorde) return;

        saliendoPared = true;
        contadorTiempoSalida = tiempoSalidaPared;
        Vector3 wallNormal = paredDer ? derWallHit.normal : izqWallHit.normal;

        Vector3 fuerza = transform.up * fuerzaUpWallJump + wallNormal * fuerzaSideWallJump;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(fuerza, ForceMode.Impulse);
    }
}
