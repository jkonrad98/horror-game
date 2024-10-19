using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : MonoBehaviour
{
    protected bool _canUseItem = true;
    [SerializeField] protected float cooldownDuration = 0.6f;

    public virtual void UseTheItem()
    {
        if (!_canUseItem)
        {
            Debug.Log("Item is on cooldown");
            return;
        }

        Debug.Log("Using parent class");

        StartCoroutine(ApplyCooldown());
    }

    protected IEnumerator ApplyCooldown()
    {
        _canUseItem = false;
        yield return new WaitForSeconds(cooldownDuration);
        _canUseItem = true;
    }
}
