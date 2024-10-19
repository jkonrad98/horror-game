using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Binoculars : UsableItem
{
    private Transform _handPivot;
    private GameObject _playerGO;
    private float _normalFOV, _desiredFOV = 30f;
    private float _zoomTransitionDuration = 0.1f;
    private PlayerCamera _playerCam;

    [Header("Zooming")]
    private CinemachineBrain _cinemachineBrain;
    private ICinemachineCamera _liveCamera;
    private CinemachineVirtualCamera virtualCam;

    [Header("Animatation")]
    private Animator _animator;

    private bool isActive = false;

    private void Start()
    {
        _playerCam = Camera.main.GetComponentInParent<PlayerCamera>();
        _normalFOV = Camera.main.fieldOfView;
        _playerGO = GameObject.FindGameObjectWithTag("Player");
        _handPivot = _playerGO.transform.Find("RightHandPivot");
        _cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        _liveCamera = _cinemachineBrain.ActiveVirtualCamera;
        transform.position = _handPivot.position;
        transform.SetParent(_handPivot);
        _animator = GetComponent<Animator>();


        UpdateLiveCamera();
        if (virtualCam != null)
        {
            _normalFOV = virtualCam.m_Lens.FieldOfView;
        }
    }

    private void UpdateLiveCamera()
    {
        _liveCamera = _cinemachineBrain.ActiveVirtualCamera;
        if (_liveCamera is CinemachineVirtualCamera cam)
        {
            virtualCam = cam;
        }
        else
        {
            Debug.LogWarning("Active camera is not a CinemachineVirtualCamera");
        }
    }
    public override void UseTheItem()
    {
        if (!_canUseItem) return;
        StartCoroutine(ApplyCooldown());
        UpdateLiveCamera();

        if (virtualCam != null)
        {
            isActive = !isActive;
            if (isActive)
            {
                _animator.SetTrigger("BinocularsOn");
            }
            else
            {
                _animator.SetTrigger("BinocularsOff");
                StartCoroutine(LerpZoomIntensity(_normalFOV));  // Start zoom out
                _playerCam.MouseMultiplier = _playerCam.BaseMultiplier;  // Reset mouse sensitivity
            }
        }

    }

    public void PutBinocularsOn()
    {
        StartCoroutine(LerpZoomIntensity(_desiredFOV));  // Start zoom in
        _playerCam.MouseMultiplier = _playerCam.ZoomMultiplier;  //Slow down sensitivity
    }

    private IEnumerator LerpZoomIntensity(float targetZoom)
    {
        if (virtualCam == null)
        {
            yield break; // If the virtual camera is null, exit the coroutine
        }

        float currentFOV = virtualCam.m_Lens.FieldOfView;
        float timeElapsed = 0f;

        while (timeElapsed < _zoomTransitionDuration)
        {
            // Interpolate FOV over time
            virtualCam.m_Lens.FieldOfView = Mathf.Lerp(currentFOV, targetZoom, timeElapsed / _zoomTransitionDuration);

            timeElapsed += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // Ensure final FOV is set
        virtualCam.m_Lens.FieldOfView = targetZoom;
    }
}

