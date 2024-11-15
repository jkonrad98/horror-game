using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [SerializeField] private bool _canBob = true;

    [SerializeField, Range(0, 0.2f)] private float _amplitude = 0.015f;
    [SerializeField, Range(0, 30f)] private float _frequency = 10f;
    [SerializeField] private float _toggleSpeed;

    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _cameraHolder;

    private Vector3 _startPos;
    private PlayerMovement _playerMovementScript;
    

    private void Awake()
    {
        _playerMovementScript = GetComponent<PlayerMovement>();
        _startPos = transform.position;
        //_camera = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        if (!_canBob) return;

        CheckMotion();
        ResetPosition();
        _camera.LookAt(FocusTarget());
    }
    private void CheckMotion()
    {
        if (_playerMovementScript == null) return;
        if (!_playerMovementScript._isGrounded) return;
        if (_playerMovementScript._flatVelocity.magnitude > 0.1f) PlayMotion(FootstepMotion());
    }

    private Vector3 FootstepMotion()
    {
        Debug.Log("Calculating");
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude * 2;
        return pos;
    }

    private void ResetPosition()
    {
        if(_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 5f * Time.deltaTime);
    }

    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
        Debug.Log("Playing Motion");

    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _cameraHolder.position.y, transform.position.z);
        pos += _cameraHolder.forward * 15f;
        return pos;
    }
}
