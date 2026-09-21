using System.Collections;
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
            GameObject obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject newObject = Instantiate(Prefab);
            return newObject;
        }
    }

    public void Return(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Attempted to return a null object to the pool.");
            return;
        }
        print(_pool);
        if (_pool.Contains(obj))
        {
            Debug.LogWarning("Attempted to return an object that is already in the pool.");
            return;
        }
        
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}
