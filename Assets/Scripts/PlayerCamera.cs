using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera")]
    public float sensX = 1f, sensY = 1f;

    Camera cam;

    float mouseX, mouseY;

    float multiplier = 1f;

    float xRot, yRot;

    private void Start()
    {
        cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        GetInput();

    }
    private void FixedUpdate()
    {
        cam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    void GetInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRot += mouseX * sensX * multiplier;
        xRot -= mouseY * sensY * multiplier;

        xRot = Mathf.Clamp(xRot, -90f, 90f);

    }
}

