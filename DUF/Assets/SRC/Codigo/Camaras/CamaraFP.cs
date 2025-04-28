using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraFP : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float rotacionX;
    float rotacionY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        rotacionY += mouseX;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotacionX, rotacionY, 0);
        orientation.rotation = Quaternion.Euler(0, rotacionY, 0);
    }
}
