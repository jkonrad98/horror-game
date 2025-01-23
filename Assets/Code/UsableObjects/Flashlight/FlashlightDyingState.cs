using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightDyingState : FlashlightBaseState
{
    private Coroutine _dyingCoroutine;

    public FlashlightDyingState(FlashlightStateManager flashlightMgr) : base(flashlightMgr) { }

    public override void EnterState()
    {
        _dyingCoroutine = _flashlightMgr.StartCoroutine(DyingBattery());
    }

    public override void UpdateState()
    {
        _flashlightMgr.DrainBattery(1f);
    }

    public override void ExitState()
    {
        if (_dyingCoroutine != null)
        {
            _flashlightMgr.StopCoroutine(_dyingCoroutine);
        }
        
    }


    private IEnumerator DyingBattery()
    {
        _flashlightMgr.SetLightActive(true);

        while (_flashlightMgr.GetBatteryLevel() > 0)
        {
            // Randomly adjust intensity to simulate flickering
            float randomIntensity = Random.Range(_flashlightMgr.GetMinFlickerIntensity() * 2f, _flashlightMgr.GetMaxFlickerIntensity());
            _flashlightMgr.SetLightIntensity(randomIntensity);

            // Wait for a random interval to simulate erratic behavior
            float waitTime = Random.Range(0.05f, 0.3f);
            yield return new WaitForSeconds(waitTime);
        }

        // Battery is depleted; switch to OffState
        _flashlightMgr.SwitchState(_flashlightMgr.offState);
    }
}
