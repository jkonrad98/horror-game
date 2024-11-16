using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightFlickerState : FlashlightBaseState
{
    private Coroutine _flickerCoroutine;

    public FlashlightFlickerState(FlashlightStateManager flashlightMgr) : base(flashlightMgr) { }

    public override void EnterState()
    {
        _flickerCoroutine = _flashlightMgr.StartCoroutine(Flicker());
    }

    public override void UpdateState()
    {
        _flashlightMgr.DrainBattery(2f);

    }

    public override void ExitState()
    {
        if (_flickerCoroutine != null)
        {
            _flashlightMgr.StopCoroutine(_flickerCoroutine);
        }
        _flashlightMgr.SetLightIntensity(_flashlightMgr.GetBaseLightIntensity());
    }
  

    private IEnumerator Flicker()
    {
        _flashlightMgr.SetLightActive(true);
        while (true)
        {
            float randomIntensity = Random.Range(_flashlightMgr.GetMinFlickerIntensity(), _flashlightMgr.GetMaxFlickerIntensity());
            _flashlightMgr.SetLightIntensity(randomIntensity);
            yield return new WaitForSeconds(_flashlightMgr.GetFlickerSpeed());
        }
    }
}
