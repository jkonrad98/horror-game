using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : MonoBehaviour
{
    public virtual void UseTheItem()
    {
        Debug.Log("Parent virtual class UsableItem");
    }
}
