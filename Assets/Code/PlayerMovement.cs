using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    private Rigidbody _rb;
    [SerializeField] private LayerMask _groundMask, _stairMask, _walkableMask;
    [SerializeField] private GameObject _stepRayUpper;
    [SerializeField] private GameObject _stepRayLower;
    [SerializeField] private Transform _rayParent;
    private PlayerInput _playerInput;

    [Header("Movement Settings")]
    [SerializeField] private float _movementBaseSpeed = 6f, _movementSpeedMultiplier = 10f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] float sprintMulti = 1.5f;
    [SerializeField] private float _jumpForce = 10f;

    [Header("Ground & Air Detection")]
    [SerializeField] private float _airMoveMulti = 0.5f;
    [SerializeField] private float _groundCheckSphereRadius = 0.3f;
    private bool _isGrounded;

    [Header("Stair & Slope Handling")]
    private RaycastHit _slopeHit;
    private Vector3 _slopeMoveDirection;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepIncrement = 0.1f;

    [Header("Drag Handling")]
    [SerializeField] private float _groundDrag = 6f, _airDrag = 0f;

    //float horizonalMovement, verticalMovement;
    //Vector3 _playerInput.MoveDirection;
    //bool _playerInput.SprintHeld;


    private float _tempY;

    private void Start()
    {
        //Rigidbody reference
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null) Debug.LogError("No input class found");

        //Stairs handling
        _stepRayUpper.transform.localPosition = new Vector3(_stepRayUpper.transform.localPosition.x, -1 + stepHeight, _stepRayUpper.transform.localPosition.z);
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
        if(_isGrounded)
        {
            _rb.drag = _groundDrag;
        }
        else
        {
            _rb.drag = _airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        GroundCheck();
        StepClimb();



        _tempY = _rb.velocity.y;
        if (_rb.velocity.magnitude > _maxSpeed)
        {
            _rb.velocity = _rb.velocity.normalized * _maxSpeed;
        }
        _rb.velocity = new Vector3(_rb.velocity.x, _tempY, _rb.velocity.z);
    }

    void StepClimb()
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
                _rb.position -= new Vector3(0f, -stepIncrement * Time.deltaTime, 0f);

            }
        }
    }


    private void MovePlayer()
    {
        if(_isGrounded && !CheckForSlopes())
        {
            if (_playerInput.SprintHeld)
            {
                _rb.AddForce(_playerInput.MoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier*sprintMulti, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed * sprintMulti, ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(_playerInput.MoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed, ForceMode.Acceleration);
            }
        }
        else if(_isGrounded && CheckForSlopes())
        {
            if (_playerInput.SprintHeld)
            {
                _rb.AddForce(_slopeMoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier * sprintMulti, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed * sprintMulti, ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(_slopeMoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed, ForceMode.Acceleration);
            }
        }
        else
        {
            if (_playerInput.SprintHeld)
            {
                _rb.AddForce(_playerInput.MoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier * sprintMulti * _airMoveMulti, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed * sprintMulti * airMoveMulti, ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(_playerInput.MoveDirection.normalized * _movementBaseSpeed * _movementSpeedMultiplier * _airMoveMulti, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed * airMoveMulti, ForceMode.Acceleration);
            }
        }
    }

    void Jump()
    {
        if (_isGrounded)
        {
            _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        }
    }

    bool CheckForSlopes()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 1.4f))
        {
            if(_slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
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

