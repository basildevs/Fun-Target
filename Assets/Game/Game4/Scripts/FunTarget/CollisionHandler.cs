using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public Action<Collider2D> OnCollision;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollision?.Invoke(collision);
        
    }
    
}
