using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovimientoJugador : MonoBehaviour
{

    [Header("Movimiento")]
    public float velocidadMove;
    public float friccionPiso;

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
    }

    // Update is called once per frame
    void Update()
    {
        tocandoPiso = Physics.Raycast(transform.position, Vector3.down, alturaJugador * (0.5f + 0.2f), piso );
        
        Inputs();

        if(tocandoPiso) {
            rb.drag = friccionPiso;
        } else {
            rb.drag = 0f;
        }


    }

    void FixedUpdate()
    {
        Movimiento();
        ControlVelocidad();
    }

    private void Inputs() {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
    }

    private void Movimiento() {

        direccionMovimiento = (orientacion.forward * inputVertical) + (orientacion.right * inputHorizontal);
        //direccionMovimiento.y = 0f;

        rb.AddForce(10f * velocidadMove * direccionMovimiento.normalized, ForceMode.Force);
    }

    private void ControlVelocidad() {
        Vector3 velPlana = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(velPlana.magnitude > velocidadMove) {
            Vector3 velLimite = velPlana.normalized * velocidadMove;
            rb.velocity = new Vector3(velLimite.x, rb.velocity.y, velLimite.z);
        }
    }
}
