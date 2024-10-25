using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _stairMask;
    [SerializeField] private LayerMask _walkableMask;
    [SerializeField] private GameObject _stepRayUpper;
    [SerializeField] private GameObject _stepRayLower;
    [SerializeField] private Transform _rayParent;
    private PlayerInput _playerInput;
    private Rigidbody _rb;

    [Header("Movement Settings")]
    [SerializeField] private float _movementBaseSpeed = 6f;
    [SerializeField] private float _movementSpeedMultiplier = 10f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _sprintMulti = 1.5f;
    [SerializeField] private float _jumpForce = 10f;
    private Vector3 _flatVelocity;
    private float _effectiveSpeed;

    [Header("Ground & Air Detection")]
    [SerializeField] private float _airMoveMulti = 0.5f;
    [SerializeField] private float _groundCheckSphereRadius = 0.3f;
    private bool _isGrounded;
    
    [Header("Stairs and Slope")]
    [SerializeField] private float _maxSlopeAngle = 45f;
    [SerializeField] private float _slopeSpeedMultiplierFactor = 50f;
    [SerializeField] private float _stepHeight = 0.3f;
    [SerializeField] private float _stepIncrement = 0.1f;
    [SerializeField] private float _maxSlopeMultiplier = 1.5f;
    private RaycastHit _slopeHit;
    private Vector3 _slopeMoveDirection;
    private float _currentSlopeAngle;
    private float _slopeMultiplier;
    private bool _isMovingUphill;
    private float _movementDirectionDot;

    [Header("Drag Handling")]
    [SerializeField] private float _groundDrag = 6f, _airDrag = 0f;

    private void Start()
    {
        //Rigidbody reference
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null) Debug.LogError("No input class found");

        //Stairs handling
        _stepRayUpper.transform.localPosition = new Vector3(_stepRayUpper.transform.localPosition.x, -1 + _stepHeight, _stepRayUpper.transform.localPosition.z);
        _walkableMask = _groundMask | _stairMask;
        
    }

    private void Update()
    {
        ControlDrag();

        _slopeMoveDirection = Vector3.ProjectOnPlane(_playerInput.MoveDirection, _slopeHit.normal);
        
        if (_playerInput.JumpPressed)
        {
            Jump();
        }
    }
    void ControlDrag()
    {
        //Set player drag to groundDrag float or AirDrag float if _isGrounded
        _rb.drag = _isGrounded ? _groundDrag : _airDrag;
    }

    private void FixedUpdate()
    {
        CalculatePlayerSpeed(); //Calculate player speed depending if InAir, Sprinting and Grounded
        GroundCheck();          //Return true if player on walkable
        MovePlayer();           //Movement logic
        StepClimb();            //Handling Stairs
        PreventSlopeSliding();  //Prevent player from sliding down the stairs      
        LimitSpeed();           //Limit maximum player speed
        ControlDrag();          //Apply drag to make the movement more feel more subtle and responsive
    }

    private void LimitSpeed()
    {
        _flatVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        if (_rb.velocity.magnitude > _maxSpeed)
        {
            Vector3 limitedVelocity = _flatVelocity.normalized * _maxSpeed;
            _rb.velocity = new Vector3(limitedVelocity.x, _rb.velocity.y, limitedVelocity.z);

        }
    }

    private void PreventSlopeSliding()
    {
        if (_isGrounded && OnSlope() && _rb.velocity.y <= 0f && _playerInput.MoveDirection == Vector3.zero)
        {
            _rb.AddForce(-_slopeHit.normal * Physics.gravity.magnitude, ForceMode.Acceleration);
            Debug.Log(Physics.gravity.magnitude);
        }
    }

    private void StepClimb()
    {
        if (_playerInput.MoveDirection == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(_playerInput.MoveDirection.normalized);
        _rayParent.rotation = targetRotation;

        Debug.DrawRay(_stepRayLower.transform.position, _rayParent.TransformDirection(Vector3.forward) * 0.25f, Color.green);
        Debug.DrawRay(_stepRayUpper.transform.position, _rayParent.TransformDirection(Vector3.forward) * 0.4f, Color.red);
        RaycastHit hitLower;
        if (Physics.Raycast(_stepRayLower.transform.position, _rayParent.TransformDirection(Vector3.forward), out hitLower, 0.25f, _stairMask))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(_stepRayUpper.transform.position, _rayParent.TransformDirection(Vector3.forward), out hitUpper, 0.4f, _stairMask))
            {
                _rb.position -= new Vector3(0f, -_stepIncrement * Time.deltaTime, 0f);

            }
        }
    }

    private void CalculatePlayerSpeed()
    {
        //Calculating movement speed in air && Sprinting
        if (!_isGrounded) _effectiveSpeed = _playerInput.SprintHeld ? (_movementBaseSpeed * _movementSpeedMultiplier * _sprintMulti * _airMoveMulti) : (_movementBaseSpeed * _movementSpeedMultiplier * _airMoveMulti);

        //Calculating movement speed on the ground
        else if (_isGrounded)
        {
            _effectiveSpeed = _playerInput.SprintHeld ? (_movementBaseSpeed * _movementSpeedMultiplier * _sprintMulti) : (_movementBaseSpeed * _movementSpeedMultiplier);
        }
    }
    
    private void MovePlayer()
    {
        if (_isGrounded)
        {
            if (OnSlope())
            {
                //Vector3 slopeDirection = Vector3.ProjectOnPlane(_playerInput.MoveDirection.normalized, _slopeHit.normal).normalized;
                float angleRad = _currentSlopeAngle * Mathf.Deg2Rad;
                float slopeMultiplier = (1f / Mathf.Cos(angleRad)) * _slopeSpeedMultiplierFactor;
                Debug.Log(angleRad);
                Debug.Log(slopeMultiplier);

                Vector3 slopeDirection = Vector3.ProjectOnPlane(_playerInput.MoveDirection.normalized, _slopeHit.normal).normalized;
                _rb.AddForce(slopeDirection * _effectiveSpeed * slopeMultiplier, ForceMode.Acceleration);

                //if (_isMovingUphill)
                //{
                //    _slopeMultiplier = 1f + (_currentSlopeAngle * _slopeSpeedMultiplierFactor);
                //    _slopeMultiplier = Mathf.Clamp(_slopeMultiplier, 1f, _maxSlopeMultiplier);

                //    _rb.AddForce(slopeDirection * _effectiveSpeed * _slopeMultiplier, ForceMode.Acceleration);
                //}
                //else
                //{
                //    _rb.AddForce(slopeDirection * _effectiveSpeed, ForceMode.Acceleration);
                //}
            }
            else
            {
                _rb.AddForce(_playerInput.MoveDirection * _effectiveSpeed, ForceMode.Acceleration);
            }
        }

        
        
        //if(_isGrounded && !OnSlope())
        //{
        //    if (_playerInput.SprintHeld)
        //    {
        //        _rb.AddForce(_playerInput.MoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier*_sprintMulti, ForceMode.Acceleration);
        //        //rb.AddRelativeForce(movementDir * moveSpeed * sprintMulti, ForceMode.Acceleration);
        //    }
        //    else
        //    {
        //        _rb.AddForce(_playerInput.MoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier, ForceMode.Acceleration);
        //        //rb.AddRelativeForce(movementDir * moveSpeed, ForceMode.Acceleration);
        //    }
        //}
        //else if(_isGrounded && OnSlope())
        //{
        //    if (_playerInput.SprintHeld)
        //    {
        //        _rb.AddForce(_slopeMoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier * _sprintMulti, ForceMode.Acceleration);
        //        //rb.AddRelativeForce(movementDir * moveSpeed * sprintMulti, ForceMode.Acceleration);
        //    }
        //    else
        //    {
        //        _rb.AddForce(_slopeMoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier, ForceMode.Acceleration);
        //        //rb.AddRelativeForce(movementDir * moveSpeed, ForceMode.Acceleration);
        //    }
        //}
        //else
        //{
        //    if (_playerInput.SprintHeld)
        //    {
        //        _rb.AddForce(_playerInput.MoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier * _sprintMulti * _airMoveMulti, ForceMode.Acceleration);
        //        //rb.AddRelativeForce(movementDir * moveSpeed * sprintMulti * airMoveMulti, ForceMode.Acceleration);
        //    }
        //    else
        //    {
        //        _rb.AddForce(_playerInput.MoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier * _airMoveMulti, ForceMode.Acceleration);
        //        //rb.AddRelativeForce(movementDir * moveSpeed * airMoveMulti, ForceMode.Acceleration);
        //    }
        //}
    }

    void Jump()
    {
        if (_isGrounded)
        {
            _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        }
    }

    bool OnSlope()
    {
        //if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.4f))
        //{
        //    if(_slopeHit.normal != Vector3.up)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //return false;
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.5f))
        {
            _currentSlopeAngle = Vector3.Angle(_slopeHit.normal, Vector3.up);

            _movementDirectionDot = Vector3.Dot(_playerInput.MoveDirection.normalized, _slopeHit.normal);
            _isMovingUphill = _movementDirectionDot > 0f;

            return _currentSlopeAngle > 0f && _currentSlopeAngle <= _maxSlopeAngle;
        }
        _currentSlopeAngle = 0f;
        return false;
    }

    void GroundCheck()
    {
        //if (Physics.Raycast(transform.position, direction, out hit, rayDist + 0.1f) && hit.collider.CompareTag("Terrain"))
        if (Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), _groundCheckSphereRadius, _walkableMask))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }
}

