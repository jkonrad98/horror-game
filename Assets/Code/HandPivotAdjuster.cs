using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPivotAdjuster : MonoBehaviour
{
    [SerializeField] private Transform _handPivot;    // Reference to your hand pivot (RightHandPivot)
    [SerializeField] private PlayerCamera _playerCamera; // Reference to your PlayerCamera script

    private void Start()
    {
        _handPivot = this.transform;
        _playerCamera = GetComponentInParent<PlayerCamera>();
    }

    void LateUpdate()
    {
        // Create a rotation from the xRot and yRot values
        Quaternion handRotation = Quaternion.Euler(_playerCamera.xRot, 0, 0);

        // Apply the rotation to the hand pivot
        _handPivot.rotation = handRotation;
    }
}
