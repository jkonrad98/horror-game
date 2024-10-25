using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Movement inputs
    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    public Vector3 MoveDirection { get; private set; }

    // Action inputs
    public bool JumpPressed { get; private set; }
    public bool SprintHeld { get; private set; }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Movement Input
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

        MoveDirection = transform.forward * VerticalInput + transform.right * HorizontalInput;
        MoveDirection = MoveDirection.normalized;

        // Jump Input
        JumpPressed = Input.GetKeyDown(KeyCode.Space);

        // Sprint Input
        SprintHeld = Input.GetKey(KeyCode.LeftShift);
    }
}
