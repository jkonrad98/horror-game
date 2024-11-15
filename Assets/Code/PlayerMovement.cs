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
    public Vector3 _flatVelocity {  get; private set; }
    private float _effectiveSpeed;

    [Header("Ground & Air Detection")]
    [SerializeField] private float _airMoveMulti = 0.5f;
    [SerializeField] private float _groundCheckSphereRadius = 0.3f;
    public bool _isGrounded { get; private set; }
    
    [Header("Stairs and Slope")]
    [SerializeField] private float _maxSlopeAngle = 45f;
    [SerializeField] private float _slopeSpeedMultiplierFactor = 1.25f;
    [SerializeField] private float _stepHeight = 0.3f;
    [SerializeField] private float _stepIncrement = 0.1f;
    [SerializeField] private float _maxSlopeMultiplier = 1.5f;
    private RaycastHit _slopeHit, _slopeHitAhead;
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
        
        if (_playerInput.JumpPressed)
        {
            Jump();
        }
    }
    private void ControlDrag()
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
            //Debug.Log(Physics.gravity.magnitude);
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

            bool isOnSlope = OnSlope();
            bool isSlopeAhead = OnSlopeAhead(out float slopeAngleAhead);
            float slopeAngleToUse = 0f;
            Vector3 slopeNormalToUse = Vector3.up;

            if (isOnSlope)
            {
                slopeAngleToUse = _currentSlopeAngle;
                slopeNormalToUse = _slopeHit.normal;
            }
            else if (isSlopeAhead)
            {
                slopeAngleToUse = slopeAngleAhead;
                slopeNormalToUse = _slopeHitAhead.normal;
            }
            if (slopeAngleToUse > 0f) //movement boost if on a slope
            {
                float angleRad = slopeAngleToUse * Mathf.Deg2Rad;
                float slopeMultiplier = (1f / Mathf.Cos(angleRad)) * _slopeSpeedMultiplierFactor;
                slopeMultiplier = Mathf.Clamp(slopeMultiplier, 1f, _maxSlopeMultiplier);

                Vector3 slopeDirection = Vector3.ProjectOnPlane(_playerInput.MoveDirection.normalized, slopeNormalToUse).normalized;
                _rb.AddForce(slopeDirection * _effectiveSpeed * slopeMultiplier, ForceMode.Acceleration);
            }
            else //movement if flat surfaces
            {
                _rb.AddForce(_playerInput.MoveDirection * _effectiveSpeed, ForceMode.Acceleration);
            }
        }
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        }
    }
    private bool OnSlopeAhead(out float slopeAngleAhead)
    {
        slopeAngleAhead = 0f;

        Vector3 origin = transform.position + Vector3.up * 0.1f;

        Vector3 direction = _playerInput.MoveDirection.normalized;
        if (direction == Vector3.zero)
        {
            return false;
        }

        float rayAngle = 45f;

        Vector3 rotationAxis = Vector3.Cross(direction, Vector3.up);
        Vector3 angledDirection = Quaternion.AngleAxis(-rayAngle, rotationAxis) * direction;


        float rayLength = 1.1f;
        Debug.DrawRay(origin, angledDirection * rayLength, Color.red);
        // Perform the raycast
        if (Physics.Raycast(origin, angledDirection, out RaycastHit hit, rayLength, _walkableMask))
        {
            slopeAngleAhead = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngleAhead > 0f && slopeAngleAhead <= _maxSlopeAngle)
            {
                _slopeHitAhead = hit;
                return true;
            }
        }

        return false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.5f))
        {
            _currentSlopeAngle = Vector3.Angle(_slopeHit.normal, Vector3.up);

            return _currentSlopeAngle > 0f && _currentSlopeAngle <= _maxSlopeAngle;
        }
        _currentSlopeAngle = 0f;
        return false;
    }

    private void GroundCheck()
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

