using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("Referencias")]
    private MovimientoJugador pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;


    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelay;

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
        pm = GetComponent<MovimientoJugador>();
        grapplingCdTimer = grapplingCd;
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(grapplingKey)) StartGrapple();
        
        if(grapplingCdTimer > 0){
            grapplingCdTimer -= Time.deltaTime;
            
        }
        
    }

    private void LateUpdate()
    {
        if (grappling){
            lr.SetPosition(0, gunTip.position);

        }
    }

    private void StartGrapple(){
        if (grapplingCdTimer > 0) return;
        grappling = true;
        pm.congelado = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable)){
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelay);
        }else{
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(EndGrapple), grappleDelay);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);

    }

    private void ExecuteGrapple(){
        pm.congelado = false;

    }


    private void EndGrapple(){
        pm.congelado = false;
        grappling = false;
        grapplingCdTimer = grapplingCd; 
        lr.enabled = false;

    }
}
