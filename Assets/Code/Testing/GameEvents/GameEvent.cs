using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent")]
public class GameEvent : ScriptableObject
{
   private List<GameEventListener> listeners = new List<GameEventListener>();

    public void Raise(Component sender, object data)
    {
        for(int i = listeners.Count - 1; i>=0; i--)
        {
            listeners[i].OnEventRaised(sender, data);
        }
    }

    public void AddListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void RemoveListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}
