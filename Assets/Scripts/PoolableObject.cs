using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    // Class for the object that can be put in the object pool
    [HideInInspector]
    public ObjectPool thePool;

    private void OnDisable()
    {
        thePool.ReturnToPool(this);
    }
}
