using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PluggablesSO : ScriptableObject
{
    //[SerializeField] protected
    public string itemName;
    public Sprite sprite;
    public Vector2 colliderDimensions;
    public Vector2 colliderOffset;

    public virtual void OnConnect()
    {
        // When a cable enters this object. 
        // Should inform game controller or similar. 
    }

    public virtual void OnDisconnect()
    {
        // When a cable leaves this object 
        // Should inform game controller or similar. 
    }
}
