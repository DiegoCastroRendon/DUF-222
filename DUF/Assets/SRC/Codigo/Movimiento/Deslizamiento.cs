using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Deslizamiento : MonoBehaviour
{
    [Header("Referencias")]
    public Transform orientacion;
    public Transform playerObj;
    private Rigidbody rb;
    private MovimientoJugador pm;

    private PlayerInput playerInput;
    private Vector2 input;

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
    private bool puedeDeslizarse = true;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<MovimientoJugador>();
        playerInput = GetComponent<PlayerInput>();

        escalaInicalY = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();

        /**
        if (Input.GetKeyDown(teclaDeslizamiento) && (input.y != 0 || input.x != 0) && puedeDeslizarse)
        {
            InicarDeslizamiento();

        }

        if(Input.GetKeyUp(teclaDeslizamiento) && pm.deslizandose) {
            DetenerDeslizamiento();
        }
        */
    }

    void FixedUpdate()
    {
        if(pm.deslizandose) {
            MovimietoDeslizamiento();
        }
    }
    public void InicarDeslizamiento(InputAction.CallbackContext callbackContext)
    {

        if (callbackContext.performed && puedeDeslizarse)
        {

            pm.deslizandose = true;
            puedeDeslizarse = false;

            rb.drag = 0f;

            playerObj.localScale = new Vector3(playerObj.localScale.x, escalaYDeslizamiento, playerObj.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            contadorDeslizamiento = tiempoMaxDeslizamiento;

            Invoke(nameof(ReiniciarDeslizamiento), coolDownDeslizamiento);
        }
        
        if (callbackContext.canceled)
        {
            DetenerDeslizamiento();
        }
        
        
    }
    private void DetenerDeslizamiento()
    {
        pm.deslizandose = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, escalaInicalY, playerObj.localScale.z);
    }

    private void ReiniciarDeslizamiento() {
        puedeDeslizarse = true;
    }
    
    public void Inputs()
    {
        // inputHorizontal = Input.GetAxisRaw("Horizontal");
        // inputVertical = Input.GetAxisRaw("Vertical");

        input = playerInput.actions["Moverse"].ReadValue<Vector2>();
    }
    private void MovimietoDeslizamiento()
    {

        Vector3 direccionInput = orientacion.forward * input.x + orientacion.right * input.y;

        if (!pm.EnPendiente() || rb.velocity.y > -0.0f)
        {

            rb.AddForce(direccionInput.normalized * fuerzaDeslizamiento, ForceMode.Force);

            contadorDeslizamiento -= Time.deltaTime;

        }
        else
        {
            rb.AddForce(pm.GetDireccionMovimientoPendiente(direccionInput) * fuerzaDeslizamiento, ForceMode.Force);
        }


        if (contadorDeslizamiento <= 0)
        {
            DetenerDeslizamiento();
        }

    }
}
