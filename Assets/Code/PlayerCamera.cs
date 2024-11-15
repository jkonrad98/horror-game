using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera")]
    public float sensX = 1f, sensY = 1f;
    public float xRot { get; set; }
    public float yRot { get; set; }
    [SerializeField] Transform _camPivot;
    public bool canRotate { get; set; }

    float mouseX, mouseY;

    private float _multiplier = 1f;
    private float _baseMultiplier = 1f, _zoomMultiplier = 0.3f;

    public float MouseMultiplier
    {
        get {  return _multiplier; }
        set { _multiplier = value; }
    }

    public float BaseMultiplier
    {
        get { return _baseMultiplier; }
    }

    public float ZoomMultiplier
    {
        get { return _zoomMultiplier; }
    }
    
    private void Start()
    {
        _multiplier = _baseMultiplier;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        GetInput();

    }
    private void FixedUpdate()
    {
 
        _camPivot.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    void GetInput()
    {
        if (!canRotate) return;
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRot += mouseX * sensX * _multiplier;
        xRot -= mouseY * sensY * _multiplier;

        xRot = Mathf.Clamp(xRot, -90f, 90f);

    }
}

