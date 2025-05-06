using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Deslizamiento : MonoBehaviour
{
    [Header("Referencias")]
    public Transform orientacion;
    public Transform playerObj;
    private Rigidbody rb;
    private MovimientoJugador pm;

    [Header("Deslizamiento")]
    public float tiempoMaxDeslizamiento;
    public float fuerzaDeslizamiento;
    private float contadorDeslizamiento;

    public float escalaYDeslizamiento;
    public float coolDownDeslizamiento;

    private float escalaInicalY;


    [Header("Controles")]
    public KeyCode teclaDeslizamiento = KeyCode.C;
    private float inputHorizontal;
    private float inputVeritical;

    private bool deslizandose;
    private bool puedeDeslizarse = true;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<MovimientoJugador>();

        escalaInicalY = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVeritical = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(teclaDeslizamiento) && (inputHorizontal != 0 || inputVeritical != 0) && puedeDeslizarse) {
            InicarDeslizamiento();

        }

        if(Input.GetKeyUp(teclaDeslizamiento) && deslizandose) {
            DetenerDeslizamiento();
        }
    }

    void FixedUpdate()
    {
        if(deslizandose) {
            MovimietoDeslizamiento();
        }
    }
    private void MovimietoDeslizamiento() {
    
        Vector3 direccionInput = orientacion.forward * inputVeritical + orientacion.right * inputHorizontal;

        
        rb.AddForce(direccionInput.normalized * fuerzaDeslizamiento, ForceMode.Force);

        contadorDeslizamiento -= Time.deltaTime;

        if(contadorDeslizamiento <= 0) {
            DetenerDeslizamiento();
        }
    
    }
    private void InicarDeslizamiento() {
        
        deslizandose = true;
        puedeDeslizarse = false;

        rb.drag = 0f;

        playerObj.localScale = new Vector3(playerObj.localScale.x, escalaYDeslizamiento, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        contadorDeslizamiento = tiempoMaxDeslizamiento;

        pm.estadoMovimieto = MovimientoJugador.EstadoMovimiento.deslizandose;

        Invoke(nameof(ReiniciarDeslizamiento), coolDownDeslizamiento);
        
    }
    private void DetenerDeslizamiento()
    {
        deslizandose = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, escalaInicalY, playerObj.localScale.z);
    }

    private void ReiniciarDeslizamiento() {
        puedeDeslizarse = true;
    }
}
