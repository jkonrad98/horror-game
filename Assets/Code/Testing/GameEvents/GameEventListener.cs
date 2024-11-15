using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public CustomGameEvent onEventTriggered;

    private void OnEnable()
    {
        gameEvent.AddListener(this); 
    }

    private void OnDisable()
    {
        gameEvent.RemoveListener(this);
    }

    //sender is for example PlayerHealth class, Data is for example health points variable in PlayerHealth class.

    public void OnEventRaised(Component sender, object data)
    {
        onEventTriggered.Invoke(sender, data);
    }
}
