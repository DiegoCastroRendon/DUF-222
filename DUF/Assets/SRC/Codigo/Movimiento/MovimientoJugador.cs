using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovimientoJugador : MonoBehaviour
{

    [Header("Movimiento")]
    public float velocidadMove;

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
        Inputs();
    }

    void FixedUpdate()
    {
        Movimiento();
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
}
