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

    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

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

    public void DoFoV(float endValue){

        GetComponent<Camera>().DOFieldOfView(endValue,0.25f);

    }

    public void DoTilt(float zTilt){

        transform.DOLocalRotate(new Vector3(0,0,zTilt), 0.25f);

    }
}
