using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }
    // Movement inputs
    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    public Vector3 MoveDirection { get; private set; }

    // Action inputs
    public bool JumpPressed { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool AltPressed { get; private set; }

    public bool LightFlickerPressed { get; private set; }
    private void Awake()
    {
        // Ensure a single instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Persist between scenes
    }
    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
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

        // Mouse Cursor Shown Input
        AltPressed = Input.GetKey(KeyCode.LeftAlt);

        LightFlickerPressed = Input.GetKeyDown(KeyCode.G);
    }
}
