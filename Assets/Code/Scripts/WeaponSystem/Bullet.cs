using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    private TrailRenderer _trailRenderer;
    private ObjectOnSphere _objectOnSphere;
    private Collider _collider;

    private void Awake()
    {
        _objectOnSphere = GetComponent<ObjectOnSphere>();
        print(_objectOnSphere);
        _trailRenderer = GetComponent<TrailRenderer>();
        _collider = GetComponent<Collider>();
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Entity") || other.CompareTag("Player"))
        {
            EntityInfo entityInfo = other.GetComponent<EntityInfo>();
            if (entityInfo != null)
            {
                entityInfo.TakeDamage(damage);
            }
            _trailRenderer.emitting = false;
            _collider.enabled = false;
            
            Invoke("Deactivate", _trailRenderer.time);
            //Destroy(gameObject, _trailRenderer.time);
        }

        if (other.CompareTag("Environment"))
        {
            _trailRenderer.emitting = false;
            _collider.enabled = false;
            Invoke("Deactivate", _trailRenderer.time);
            //Destroy(gameObject, _trailRenderer.time);
        }
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Initialize(int damage, float velocity, Vector3 direction)
    {
        this.damage = damage;
        _objectOnSphere.SetVelocity(direction * velocity);
    }

}
