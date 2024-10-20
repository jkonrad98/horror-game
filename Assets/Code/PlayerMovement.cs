using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f, moveSpeedMulti = 10f, jumpForce = 10f, airMoveMulti = 0.5f;
    float groundDrag = 6f, airDrag = 0f, groundCheckSphereRadius = 0.3f;
    float horizonalMovement, verticalMovement;
    Vector3 moveDir;
    [SerializeField] LayerMask groundMask, stairMask, walkableMask;
    RaycastHit slopeHit;
    Vector3 slopeMoveDirection;



    public float maxSpeed = 10f;
    public float tempY;

    [Header("Handling Stairs")]
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight = 0.3f;
    [SerializeField] float stepIncrement = 0.1f;
    [SerializeField] Transform rayParent;


    [Header("AirDetection")]
    float rayDist = 1f;
    bool isGrounded;

    [Header("Sprint")]
    [SerializeField] float sprintMulti = 1.5f;
    bool isSprinting;

    private void Start()
    {
        //Rigidbody reference
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //Stairs handling
        stepRayUpper.transform.localPosition = new Vector3(stepRayUpper.transform.localPosition.x, -1 + stepHeight, stepRayUpper.transform.localPosition.z);
        walkableMask = groundMask | stairMask;
        
    }

    private void Update()
    {
        MyInput();
        ControlDrag();

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);

    }

    void MyInput()
    {
        //Movement Input
        horizonalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        //movementDir = new Vector3(horizonalMovement, 0, verticalMovement);

        moveDir = transform.forward * verticalMovement + transform.right * horizonalMovement;

        //Inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }


        if(Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    void ControlDrag()
    {
        if(isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        GroundCheck();
        StepClimb();

        tempY = rb.velocity.y;
        if(rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        rb.velocity = new Vector3(rb.velocity.x, tempY, rb.velocity.z);
    }

    void StepClimb()
    {
        if (moveDir == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(moveDir.normalized);
        rayParent.rotation = targetRotation;

        Debug.DrawRay(stepRayLower.transform.position, rayParent.TransformDirection(Vector3.forward) * 0.25f, Color.green);
        Debug.DrawRay(stepRayUpper.transform.position, rayParent.TransformDirection(Vector3.forward) * 0.4f, Color.red);
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, rayParent.TransformDirection(Vector3.forward), out hitLower, 0.25f, stairMask))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, rayParent.TransformDirection(Vector3.forward), out hitUpper, 0.4f, stairMask))
            {
                rb.position -= new Vector3(0f, -stepIncrement * Time.deltaTime, 0f);

            }
        }
    }


    private void MovePlayer()
    {
        if(isGrounded && !CheckForSlopes())
        {
            if (isSprinting)
            {
                rb.AddForce(moveDir.normalized * moveSpeed * moveSpeedMulti*sprintMulti, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed * sprintMulti, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(moveDir.normalized * moveSpeed * moveSpeedMulti, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed, ForceMode.Acceleration);
            }
        }
        else if(isGrounded && CheckForSlopes())
        {
            if (isSprinting)
            {
                rb.AddForce(slopeMoveDirection.normalized * moveSpeed * moveSpeedMulti * sprintMulti, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed * sprintMulti, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(slopeMoveDirection.normalized * moveSpeed * moveSpeedMulti, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed, ForceMode.Acceleration);
            }
        }
        else
        {
            if (isSprinting)
            {
                rb.AddForce(moveDir.normalized * moveSpeed * moveSpeedMulti * sprintMulti * airMoveMulti, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed * sprintMulti * airMoveMulti, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(moveDir.normalized * moveSpeed * moveSpeedMulti * airMoveMulti, ForceMode.Acceleration);
                //rb.AddRelativeForce(movementDir * moveSpeed * airMoveMulti, ForceMode.Acceleration);
            }
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    bool CheckForSlopes()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.4f))
        {
            if(slopeHit.normal != Vector3.up)
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
        if (Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundCheckSphereRadius, walkableMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}

