using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject Prefab;
    [SerializeField] private Queue<GameObject> _pool = new Queue<GameObject>();

    public GameObject Get()
    {
        if (_pool.Count > 0)
        {
            print("Before dequeue :" + _pool.Count);
            GameObject obj = _pool.Dequeue();
            print("After dequeue :" + _pool.Count);
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject newObject = Instantiate(Prefab);
            //_pool.Enqueue(newObject);
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

    private void Update()
    {
        
    }
}
