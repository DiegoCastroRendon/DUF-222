using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{

    [Header("Wallrunning")]
    public LayerMask esPared;
    public LayerMask esSuelo;
    public float fuerzaWallRun;
    public float velocidadSubirPared;
    public float tiempoMaximoWallRun;
    private float contadorWallRun;
    [Header("Controles")]
    public KeyCode WallRunArriba = KeyCode.I;
    public KeyCode WallRunAbajo = KeyCode.K;
    private bool runArriba;
    private bool runAbajo;
    private float horizontalInput;
    private float verticalInput;
    [Header ("Deteccion")]
    public float checaDistanciaPared;
    public float minAlturaSalto;
    private RaycastHit izqWallHit;
    private RaycastHit derWallHit;
    private bool paredIzq;
    private bool paredDer;
    [Header("Referencias")]
    public Transform orientacion;
    private MovimientoJugador pm;
    private Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<MovimientoJugador>();
    }

    // Update is called once per frame
    void Update()
    {
        VerificaPared();
        Estados();
    }

    void FixedUpdate(){
        if(pm.wallrunning){
            WallRunMov();
        }
    }

    private void VerificaPared(){
        paredDer = Physics.Raycast(transform.position, orientacion.right, out derWallHit, checaDistanciaPared, esPared);
        paredIzq = Physics.Raycast(transform.position, -orientacion.right, out izqWallHit, checaDistanciaPared, esPared);

    }

    private bool SobreSuelo(){
        return !Physics.Raycast(transform.position, Vector3.down, minAlturaSalto, esSuelo);
    }

    private void Estados(){

        //Controles
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        runArriba = Input.GetKey(WallRunArriba);
        runAbajo = Input.GetKey(WallRunAbajo);

        // Estado de wallrun
        if((paredIzq || paredDer) && verticalInput > 0 && SobreSuelo()){
            //Empezar wallrun
            if(!pm.wallrunning){
                StartWallRun();
            }

        }else{
            
            if(pm.wallrunning){
                StopWallRun();
            }
        }
    }

    private void StartWallRun(){
        pm.wallrunning = true;

    }

    private void WallRunMov(){

        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = paredDer ? derWallHit.normal : izqWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        

        if((orientacion.forward - wallForward).magnitude > (orientacion.forward - -wallForward).magnitude){
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * fuerzaWallRun, ForceMode.Force);

        if(runArriba){
            rb.velocity = new Vector3(rb.velocity.x, velocidadSubirPared, rb.velocity.z); 
        }
        if(runAbajo){
            rb.velocity = new Vector3(rb.velocity.x, -velocidadSubirPared, rb.velocity.z); 
        }

        //Empujar a la pared
        if(!(paredIzq && horizontalInput > 0) && !(paredDer && horizontalInput < 0)){
            rb.AddForce(-wallNormal * 100, ForceMode.Force);  
        }
          


    }

    private void StopWallRun(){
        pm.wallrunning = false;

    }
}
