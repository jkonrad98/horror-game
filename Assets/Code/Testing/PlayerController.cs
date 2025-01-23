using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _stairMask;
    [SerializeField] private LayerMask _walkableMask;
    [SerializeField] private GameObject _stepRayUpper;
    [SerializeField] private GameObject _stepRayLower;
    [SerializeField] private Transform _rayParent;
    private PlayerInput _playerInput;
    private CharacterController _controller;

    [Header("Movement Settings")]
    [SerializeField] private float _movementBaseSpeed = 6f;
    [SerializeField] private float _movementSpeedMultiplier = 10f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _sprintMulti = 1.5f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _gravity = -9.81f;
    public Vector3 _flatVelocity { get; private set; }
    private float _effectiveSpeed;

    [Header("Ground & Air Detection")]
    [SerializeField] private float _airMoveMulti = 0.5f;
    [SerializeField] private float _groundCheckSphereRadius = 0.3f;
    public bool _isGrounded { get; private set; }
    private Vector3 _velocity;

    //[Header("Stairs and Slope")]
    //[SerializeField] private float _maxSlopeAngle = 45f;
    //[SerializeField] private float _slopeSpeedMultiplierFactor = 1.25f;
    //[SerializeField] private float _stepHeight = 0.3f;
    //[SerializeField] private float _stepIncrement = 0.1f;
    //[SerializeField] private float _maxSlopeMultiplier = 1.5f;
    //private RaycastHit _slopeHit, _slopeHitAhead;
    //private Vector3 _slopeMoveDirection;
    //private float _currentSlopeAngle;
    //private float _slopeMultiplier;
    //private bool _isMovingUphill;

    private void Start()
    {
        // Initialize CharacterController
        _gravity = Physics.gravity.y;
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();

        if (_playerInput == null) Debug.LogError("No input class found");
        _walkableMask = _groundMask | _stairMask;
    }
    private void Update()
    {
        GroundCheck();
        CalculatePlayerSpeed();
        if (_playerInput.JumpPressed && _isGrounded)
        {
            Jump();
        }
    }
    private void FixedUpdate()
    {      
        MovePlayer();
        ApplyGravity();
    }

    private void GroundCheck()
    {
        _isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), _groundCheckSphereRadius, _walkableMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // Reset vertical velocity when grounded
        }
    }

    private void CalculatePlayerSpeed()
    {
        if (!_isGrounded)
        {
            _effectiveSpeed = _playerInput.SprintHeld
                ? (_movementBaseSpeed * _movementSpeedMultiplier * _sprintMulti * _airMoveMulti)
                : (_movementBaseSpeed * _movementSpeedMultiplier * _airMoveMulti);
        }
        else
        {
            _effectiveSpeed = _playerInput.SprintHeld
                ? (_movementBaseSpeed * _movementSpeedMultiplier * _sprintMulti)
                : (_movementBaseSpeed * _movementSpeedMultiplier);
        }
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = _playerInput.MoveDirection * _effectiveSpeed;

        //if (OnSlope())
        //{
        //    Vector3 slopeDirection = Vector3.ProjectOnPlane(moveDirection, _slopeHit.normal).normalized;
        //    float slopeMultiplier = Mathf.Clamp(
        //        (1f / Mathf.Cos(_currentSlopeAngle * Mathf.Deg2Rad)) * _slopeSpeedMultiplierFactor,
        //        1f,
        //        _maxSlopeMultiplier
        //    );

            //moveDirection = slopeDirection * _effectiveSpeed * slopeMultiplier;
        //}

        _controller.Move((moveDirection + _velocity) * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        _velocity.y += _gravity * Time.deltaTime;
    }

    private void Jump()
    {
        _velocity.y = Mathf.Sqrt(_jumpForce * -2f * _gravity);
    }

    //private bool OnSlope()
    //{
    //    if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.5f))
    //    {
    //        _currentSlopeAngle = Vector3.Angle(_slopeHit.normal, Vector3.up);
    //        return _currentSlopeAngle > 0f && _currentSlopeAngle <= _maxSlopeAngle;
    //    }

    //    _currentSlopeAngle = 0f;
    //    return false;
    //}
}