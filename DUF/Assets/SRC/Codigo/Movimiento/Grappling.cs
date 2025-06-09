using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Permite al jugador engancharse a superficies con una cuerda y ser impulsado hacia ellas.
/// </summary>
public class Grappling : MonoBehaviour
{
    [Header("Referencias")]
    private MovimientoJugador pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
    private PlayerInput playerInput;


    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelay;
    public float overshootY;



    private Vector3 grapplePoint;


    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;
    private bool grappling;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        pm = GetComponent<MovimientoJugador>();
        grapplingCdTimer = grapplingCd;

    }

    // Update is called once per frame
    void Update()
    {


        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;

        }

    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);

        }
    }

    /// <summary>
    /// Inicia el intento de enganche cuando se presiona la tecla asignada.
    /// </summary>
    public void StartGrapple(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started)
        {
            if (grapplingCdTimer > 0) return;
            grappling = true;
            pm.congelado = true;

            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
            {
                grapplePoint = hit.point;

                Invoke(nameof(ExecuteGrapple), grappleDelay);
            }
            else
            {
                grapplePoint = cam.position + cam.forward * maxGrappleDistance;
                Invoke(nameof(EndGrapple), grappleDelay);
            }

            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);
        }

    }

    /// <summary>
    /// Ejecuta el impulso hacia el punto de enganche.
    /// </summary>
    private void ExecuteGrapple()
    {
        pm.congelado = false;

        Vector3 lowGrapplePoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeY = grapplePoint.y - lowGrapplePoint.y;
        float highPoint = grapplePointRelativeY + overshootY;
        if (grapplePointRelativeY < 0) highPoint = overshootY;
        pm.JumpToPosition(grapplePoint, highPoint);
        Invoke(nameof(EndGrapple), 1f);

    }


    /// <summary>
    /// Finaliza el proceso del gancho y reinicia su cooldown.
    /// </summary>
    public void EndGrapple()
    {
        pm.congelado = false;
        grappling = false;
        grapplingCdTimer = grapplingCd;
        lr.enabled = false;

    }
}
