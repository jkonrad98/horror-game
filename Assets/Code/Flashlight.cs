using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Flashlight : UsableItem
{
    private Vector3 _offsetVector;
    private Transform _targetToFollow, _handPivot;
    private GameObject _playerGO;
    [SerializeField] private float offsetSpeed = 4f;
    [SerializeField] private Light flashlightSpotlight;
    private float _baseLightStrength;
    // Start is called before the first frame update
    [SerializeField] private float lerpDuration = 0.1f;
    bool isActive = true;

    void Start()
    {
        flashlightSpotlight = GetComponentInChildren<Light>();
        _targetToFollow = Camera.main.transform;
        _playerGO = GameObject.FindGameObjectWithTag("Player");
        _handPivot = _playerGO.transform.Find("RightHandPivot");
        _baseLightStrength = flashlightSpotlight.intensity;

        transform.position = _handPivot.position;
        transform.rotation = _handPivot.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _handPivot.position, offsetSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(_targetToFollow.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, offsetSpeed * Time.deltaTime);
    }

    public override void UseTheItem()
    {

        isActive = !isActive;

        // Stop any current lerp in progress
        StopAllCoroutines();
        

        // Start the intensity lerp coroutine
        StartCoroutine(LerpLightIntensity(isActive ? 5f : 0f));

    }

    private IEnumerator LerpLightIntensity(float targetIntensity)
    {
        float startIntensity = flashlightSpotlight.intensity;
        float timeElapsed = 0f;

        while (timeElapsed < lerpDuration)
        {
            // Linearly interpolate the intensity based on the elapsed time
            flashlightSpotlight.intensity = Mathf.Lerp(startIntensity, targetIntensity, timeElapsed / lerpDuration);

            // Increment elapsed time
            timeElapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;
            
        }

        // Ensure the final intensity is exactly the target value
        flashlightSpotlight.intensity = targetIntensity;
    }
}
