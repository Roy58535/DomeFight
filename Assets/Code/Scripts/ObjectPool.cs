using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject Prefab;
    private Queue<GameObject> _pool = new Queue<GameObject>();

    public GameObject Get()
    {
        if (_pool.Count > 0)
        {
            // Dequeue an object from the pool and activate it
            GameObject obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // If the pool is empty, instantiate a new object
            GameObject newObject = Instantiate(Prefab);
            return newObject;
        }
    }

    public void Return(GameObject obj)
    {
        if (obj == null)
        {
            // Warn if attempting to return a null object to the pool
            Debug.LogWarning("Attempted to return a null object to the pool.");
            return;
        }
        if (_pool.Contains(obj))
        {
            // Warn if attempting to return an object that is already in the pool
            Debug.LogWarning("Attempted to return an object that is already in the pool.");
            return;
        }

        // Enque the object back into the pool and deactivate it
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}
