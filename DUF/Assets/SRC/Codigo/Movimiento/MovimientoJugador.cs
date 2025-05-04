using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovimientoJugador : MonoBehaviour
{

    [Header("Movimiento")]
    public float velocidadMove;
    public float friccionPiso;
    public float fuarzaSalto;
    public float coolDownSalto;
    public float multiplicadorAire;
    bool puedeSaltar;

    [Header("Controles")]
    public KeyCode teclaSalto = KeyCode.Space;

    [Header("Check piso")]
    public float alturaJugador;
    public LayerMask piso;
    bool tocandoPiso;

    public Transform orientacion;

    float inputHorizontal;
    float inputVertical;

    Vector3 direccionMovimiento;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;
        rb.mass = 1f;

        puedeSaltar = true;
    }

    // Update is called once per frame
    void Update()
    {

        tocandoPiso = Physics.Raycast(transform.position, Vector3.down, alturaJugador * 0.5f + 0.2f, piso);

        Debug.DrawRay(transform.position, Vector3.down * (alturaJugador * 0.5f + 0.2f), Color.red);

        Inputs();

        if(tocandoPiso) {
            rb.drag = friccionPiso;
        }

        Debug.Log("TocandoPiso: " + tocandoPiso);
        
    }

    void FixedUpdate()
    {
        Movimiento();
        ControlVelocidad();
        rb.AddForce(Vector3.down * 9.81f * rb.mass, ForceMode.Force);

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
    }

    private void Movimiento() {

        direccionMovimiento = (orientacion.forward * inputVertical) + (orientacion.right * inputHorizontal);

        
        //direccionMovimiento.y = 0f;


        if(tocandoPiso)
            rb.AddForce(10f * velocidadMove * direccionMovimiento.normalized, ForceMode.Force);

        else if (!tocandoPiso)
            rb.AddForce(10f * multiplicadorAire * velocidadMove * direccionMovimiento.normalized, ForceMode.Force);



    }

    private void ControlVelocidad() {
        Vector3 velPlana = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(velPlana.magnitude > velocidadMove) {
            Vector3 velLimite = velPlana.normalized * velocidadMove;
            rb.velocity = new Vector3(velLimite.x, rb.velocity.y, velLimite.z);
        }
    }

    private void Salto() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * fuarzaSalto, ForceMode.Impulse);
    }

    private void ResetSalto() {
        puedeSaltar = true;
    }
}
