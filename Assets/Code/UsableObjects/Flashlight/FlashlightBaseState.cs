using UnityEngine;

public abstract class FlashlightBaseState
{
    protected FlashlightStateManager _flashlightMgr;

    public FlashlightBaseState(FlashlightStateManager flashlightMgr)
    {
        this._flashlightMgr = flashlightMgr;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();

    public virtual void HandleInput() { }
}
