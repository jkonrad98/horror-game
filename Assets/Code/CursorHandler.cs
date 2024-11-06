using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    private PlayerInput _pInput;
    private PlayerCamera _cameraScript;

    private void Awake()
    {
        _pInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        _cameraScript = _pInput.gameObject.GetComponent<PlayerCamera>();
    }
    private void Update()
    {
        CursorShown();
    }
    private void CursorShown()
    {
        if (_pInput == null) return;
        if (_cameraScript == null) return;

        if (!_pInput.AltPressed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _cameraScript.canRotate = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            _cameraScript.canRotate = false;

        }
    }
}
