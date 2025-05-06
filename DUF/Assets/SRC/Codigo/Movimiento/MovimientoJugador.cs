using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovimientoJugador : MonoBehaviour
{

    [Header("Movimiento")]
    private float velocidadMovimiento;
    public float velocidadCaminando;
    public float velocidadCorriendo;
    public float friccionPiso;

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
    bool tocandoPiso;

    public Transform orientacion;

    float inputHorizontal;
    float inputVertical;

    [Header("Manejador de pendientes")]
    public float maxAunguloPendiente;
    private RaycastHit tocandoPendiente;
    private bool saliendoPendiente;

    Vector3 direccionMovimiento;

    Rigidbody rb;

    public EstadoMovimiento estadoMovimieto;

    public enum EstadoMovimiento {
        caminando,
        corriendo,
        agachado,
        aire
    }

    // Start is called before the first frame update
    void Start()
    {
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

        if(tocandoPiso) {
            rb.drag = friccionPiso;
        } else {
            rb.drag = 0f;
        }

        Debug.Log("TocandoPiso: " + tocandoPiso);
        
    }

    void FixedUpdate()
    {
        Movimiento();
        rb.AddForce(9.81f * rb.mass * Vector3.down, ForceMode.Force);

    }

    private void Inputs() {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(teclaSalto))
        {
            Debug.Log("Tecla de salto presionada");
        }

        if (Input.GetKey(teclaSalto) && puedeSaltar && tocandoPiso) {
            puedeSaltar = false;
            Salto();
            Invoke(nameof(ResetSalto), coolDownSalto);
        }

        if (Input.GetKeyDown(teclaAgacharse) ) {
            transform.localScale = new Vector3(transform.localScale.x, escalaYAgachado, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(teclaAgacharse) ) {
            transform.localScale = new Vector3(transform.localScale.x, inicioEscalY, transform.localScale.z);
        }
    }

    private void Movimiento() {

        direccionMovimiento = (orientacion.forward * inputVertical) + (orientacion.right * inputHorizontal);

        if(EnPendiente() && !saliendoPendiente) {
            rb.AddForce(20f * velocidadMovimiento * GetDireccionMovimientoPendiente(), ForceMode.Force);

            if (rb.velocity.y > 0f) 
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }


        if(tocandoPiso)
            rb.AddForce(10f * velocidadMovimiento * direccionMovimiento.normalized, ForceMode.Force);

        else if (!tocandoPiso)
            rb.AddForce(10f * multiplicadorAire * velocidadMovimiento * direccionMovimiento.normalized, ForceMode.Force);



    }

    private void ControlVelocidad() {

        if(EnPendiente() && !saliendoPendiente) {
            if(rb.velocity.magnitude > velocidadMovimiento) {
                rb.velocity = rb.velocity.normalized * velocidadMovimiento;
            }
        } else  {
            Vector3 velPlana = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if(velPlana.magnitude > velocidadMovimiento) {
                Vector3 velLimite = velPlana.normalized * velocidadMovimiento;
                rb.velocity = new Vector3(velLimite.x, rb.velocity.y, velLimite.z);
            }
        }
    }

    private void Salto() {
        saliendoPendiente = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * fuarzaSalto, ForceMode.Impulse);
    }

    private void ResetSalto() {
        puedeSaltar = true;

        saliendoPendiente = false;
    }

    private void ManejadorEstadoMovimiento() {
        if(Input.GetKey(teclaAgacharse)) {
            estadoMovimieto = EstadoMovimiento.agachado;
            velocidadMovimiento = velocidadAgachado;
        }

        if(tocandoPiso && Input.GetKey(teclaCorrer)){
            estadoMovimieto = EstadoMovimiento.corriendo;
            velocidadMovimiento = velocidadCorriendo;
        }

        else if(tocandoPiso) {
            estadoMovimieto = EstadoMovimiento.caminando;
            velocidadMovimiento = velocidadCaminando;
        }

        else {
            estadoMovimieto = EstadoMovimiento.aire;
        }
    }

    private bool EnPendiente() {
        if(Physics.Raycast(transform.position, Vector3.down, out tocandoPendiente, alturaJugador * 0.5f + 0.3f))
         {
            float anguloPendiente = Vector3.Angle(Vector3.up, tocandoPendiente.normal);
            return anguloPendiente < maxAunguloPendiente && anguloPendiente != 0f;
        }

        return false;
    }

    private Vector3 GetDireccionMovimientoPendiente() {
        return Vector3.ProjectOnPlane(direccionMovimiento, tocandoPendiente.normal).normalized;
    }
}
