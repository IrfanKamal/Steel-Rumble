using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    [HideInInspector]
    public ObjectPool thePool;

    private void OnDisable()
    {
        thePool.ReturnToPool(this);
    }
}
