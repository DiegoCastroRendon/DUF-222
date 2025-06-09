using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;


public class CamaraFP : MonoBehaviour
{
    public float sensX;
    public float sensY;
    private PlayerInput playerInput;
    private Vector2 input;

    public Transform orientation;
    public Transform camHolder;

    float rotacionX;
    float rotacionY;

    /// <summary>
    /// Inicializa el componente PlayerInput y bloquea el cursor al centro de la pantalla.
    /// </summary>
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Lee la entrada de la cámara, actualiza la rotación de la cámara y la orientación del jugador.
    /// </summary>
    void Update()
    {
        input = playerInput.actions["Camara"].ReadValue<Vector2>();

        float mouseX = input.x;
        float mouseY = input.y;

        //float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        //float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        rotacionY += mouseX;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(rotacionX, rotacionY, 0);
        orientation.rotation = Quaternion.Euler(0, rotacionY, 0);
    }

    /// <summary>
    /// Realiza una animación de cambio de Field of View (FoV) de la cámara.
    /// </summary>
    /// <param name="endValue">Valor final del Field of View.</param>
    public void DoFoV(float endValue)
    {

        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);

    }

    /// <summary>
    /// Realiza una animación de inclinación (tilt) de la cámara en el eje Z.
    /// </summary>
    /// <param name="zTilt">Ángulo de inclinación en el eje Z.</param>
    public void DoTilt(float zTilt)
    {

        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);

    }
}
