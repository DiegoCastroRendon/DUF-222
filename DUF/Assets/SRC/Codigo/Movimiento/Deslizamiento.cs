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



    /// <summary>
    /// Inicializa las referencias y variables necesarias para el deslizamiento.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<MovimientoJugador>();
        playerInput = GetComponent<PlayerInput>();

        escalaInicalY = playerObj.localScale.y;
    }

    /// <summary>
    /// Llama a Inputs() y gestiona la lógica de entrada para el deslizamiento.
    /// </summary>
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

    /// <summary>
    /// Aplica la física del deslizamiento si el jugador está deslizándose.
    /// </summary>
    void FixedUpdate()
    {
        if(pm.deslizandose) {
            MovimietoDeslizamiento();
        }
    }
    
    /// <summary>
    /// Inicia el deslizamiento si la entrada es válida y se puede deslizar.
    /// </summary>
    /// <param name="callbackContext">Contexto de la acción de entrada.</param>
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

    /// <summary>
    /// Detiene el deslizamiento y restablece la escala del jugador.
    /// </summary>
    private void DetenerDeslizamiento()
    {
        pm.deslizandose = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, escalaInicalY, playerObj.localScale.z);
    }

    /// <summary>
    /// Permite que el jugador pueda volver a deslizarse después del cooldown.
    /// </summary>
    private void ReiniciarDeslizamiento()
    {
        puedeDeslizarse = true;
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
    /// Aplica la fuerza y lógica de movimiento durante el deslizamiento.
    /// </summary>
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
