using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Flashlight : MonoBehaviour
{
    private Vector3 offsetVector;
    private Transform targetToFollow;
    [SerializeField] private float offsetSpeed = 4f;
    [SerializeField] private Light flashlightSpotlight;
    private float _baseLightStrength;
    // Start is called before the first frame update
    private float lerpDuration = 0.1f;
    bool isActive = true;

    void Start()
    {
        flashlightSpotlight = GetComponentInChildren<Light>();
        targetToFollow = Camera.main.transform;
        offsetVector = transform.position - targetToFollow.position;
        _baseLightStrength = flashlightSpotlight.intensity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = targetToFollow.position + offsetVector;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetToFollow.rotation, offsetSpeed * Time.deltaTime);

    }

    public void UseTheItem()
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
