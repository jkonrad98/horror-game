using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPivotAdjuster : MonoBehaviour
{
    [SerializeField] private Transform _handPivot;
    [SerializeField] private Transform _cameraTransform;
   

    void Start()
    {
        if (_handPivot == null)
        {
            _handPivot = this.transform;
        }
        if (_cameraTransform == null)
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    void FixedUpdate()
    {
        float cameraPitch = _cameraTransform.eulerAngles.x;

        // Convert pitch to range between -180 and 180 if needed
        if (cameraPitch > 180f)
        {
            cameraPitch -= 360f;
        }

        // Rotate the hand pivot only on the X axis, keeping its relative position to the player
        _handPivot.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
    }
}
