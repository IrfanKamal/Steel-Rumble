using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // Class for create and controlling object pooling

    // variables
    public string objectName;
    [SerializeField]
    public List<PoolableObject> gameObjects;
    Stack<PoolableObject> objects;
    public GameObject prefab;

    private void Start()
    {
        objects = new Stack<PoolableObject>();
        for (int i = 0; i < gameObjects.Count; i++)
        {
            objects.Push(gameObjects[i]);
        }
    }

    // When a class request the object
    public GameObject RequestObject()
    {
        PoolableObject requested;
        if (objects.Count > 0)
        {
            requested = objects.Pop();
        }
        else
        {
            GameObject gameObject = Instantiate(prefab);
            gameObject.SetActive(false);
            requested = gameObject.GetComponent<PoolableObject>();
        }
        requested.thePool = this;
        return requested.gameObject;
    }

    // When the object done being used
    public void ReturnToPool(PoolableObject gameObject)
    {
        objects.Push(gameObject);
    }

}
