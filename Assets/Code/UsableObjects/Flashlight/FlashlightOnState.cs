using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightOnState : FlashlightBaseState
{
    private Coroutine _intensityLerpCoroutine;

    public FlashlightOnState(FlashlightStateManager flashlightMgr) : base(flashlightMgr) { }

    public override void EnterState()
    {
        _intensityLerpCoroutine = _flashlightMgr.StartCoroutine(LerpLightIntensity(_flashlightMgr.GetBaseLightIntensity()));
    }

    public override void UpdateState()
    {
        _flashlightMgr.DrainBattery(1f);
    }

    public override void ExitState()
    {
        if (_intensityLerpCoroutine != null)
        {
            _flashlightMgr.StopCoroutine(_intensityLerpCoroutine);
        }
    }

    private IEnumerator LerpLightIntensity(float targetIntensity)
    {
        float startIntensity = _flashlightMgr.GetLightIntensity();
        float timeElapsed = 0f;
        float lerpDuration = _flashlightMgr.GetLerpDuration();

        while (timeElapsed < lerpDuration)
        {
            // Linearly interpolate the intensity based on the elapsed time
            _flashlightMgr.SetLightIntensity(Mathf.Lerp(startIntensity, targetIntensity, timeElapsed / lerpDuration));

            // Increment elapsed time
            timeElapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;

        }

        // Ensure the final intensity is exactly the target value
        _flashlightMgr.SetLightIntensity(targetIntensity);
    }
}
