using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightStateManager : UsableItem
{
    [Header("References")]
    private Transform _targetToFollow, _handPivot;
    private GameObject _playerGO;
    [SerializeField] private Light _flashlightLight;

    [Header("Light Settings")]
    private Vector3 _offsetVector;
    [SerializeField] private float _offsetSpeed = 4f;
    [SerializeField] private float _lerpDuration = 0.1f;
    private bool _isActive = true;

    [Header("Flicker Settings")]
    [SerializeField, Range(0.1f, 1f)] private float _minIntensity = 0.5f;
    [SerializeField, Range(1f, 10f)] private float _maxIntensity = 1.0f;
    [SerializeField, Range(0.05f, 0.25f)] private float _flickerSpeed = 0.1f;

    private float _baseLightStrength;

    // State references
    [HideInInspector] public FlashlightBaseState currentState;
    [HideInInspector] public FlashlightOffState offState;
    [HideInInspector] public FlashlightOnState onState;
    [HideInInspector] public FlashlightFlickerState flickerState;

    void Awake()
    {
        // Get references
        _flashlightLight = GetComponentInChildren<Light>();
        _targetToFollow = Camera.main.transform;
        _playerGO = GameObject.FindGameObjectWithTag("Player");
        _handPivot = GameObject.FindGameObjectWithTag("Hand").transform;
        _baseLightStrength = _flashlightLight.intensity;

        // Set flashlight position to the hand
        transform.position = _handPivot.position;
        transform.rotation = _handPivot.rotation;

        // Initialize states
        offState = new FlashlightOffState(this);
        onState = new FlashlightOnState(this);
        flickerState = new FlashlightFlickerState(this);
    }
    public override void UseTheItem()
    {
        if(currentState == offState)
        {
            SwitchState(onState);
        }

        else if(currentState == onState)
        {
            SwitchState(offState);
        }
        //// Stop any current lerp in progress
        //StopAllCoroutines();


        //// Start the intensity lerp coroutine
        //StartCoroutine(LerpLightIntensity(_isActive ? 5f : 0f));

    }
    void Start()
    {
        // Set initial state
        SwitchState(offState);
    }
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _handPivot.position, _offsetSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(_targetToFollow.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _offsetSpeed * Time.deltaTime);
    }

    void Update()
    {
        // Delegate update and input handling to the current state
        currentState.UpdateState();
        currentState.HandleInput();

        if (PlayerInput.Instance.LightFlickerPressed)
        {
            if (currentState != flickerState)
            {
                SwitchState(flickerState);
                currentState = flickerState;
            }
            else
            {
                // Return to previous state or default to On state
                SwitchState(onState);
                currentState = onState;
            }
        }

    }

    public void SwitchState(FlashlightBaseState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }
        currentState = newState;
        currentState.EnterState();
    }

    #region Setters
    public void SetLightActive(bool isActive)
    {
        _flashlightLight.enabled = isActive;
    }

    public void SetLightIntensity(float intensity)
    {
        _flashlightLight.intensity = intensity;
    }
    #endregion

    #region Getters
    public float GetMaxFlickerIntensity()
    {
        return _maxIntensity;
    }
    public float GetMinFlickerIntensity()
    {
        return _minIntensity;
    }
    public float GetFlickerSpeed()
    {
        return _flickerSpeed;
    }
    public float GetBaseLightIntensity()
    {
        return _baseLightStrength;
    }
    public float GetLightIntensity()
    {
        return _flashlightLight.intensity;
    }
    public float GetLerpDuration()
    {
        return _lerpDuration;
    }
    #endregion
}
