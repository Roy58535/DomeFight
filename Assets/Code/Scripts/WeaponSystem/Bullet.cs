using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ObjectOnSphere))]
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]

public class Bullet : MonoBehaviour
{
    private int _damage;
    private TrailRenderer _trailRenderer;
    private ObjectOnSphere _objectOnSphere;
    private SphereCollider _sphereCollider;
    private Vector3 _previousCenter;
    private bool _initialized;
    private ObjectPool _bulletPool;

    private void Awake()
    {
        _objectOnSphere = GetComponent<ObjectOnSphere>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    //private void Start()
    //{
    //    Initialize(null, null, 0f, Vector3.zero, 0);
    //}

    private void FixedUpdate()
    {
        // Sweep test for environment collision
        if (!_initialized)
            return;

        Vector3 currentCenter = transform.position;
        Vector3 displacement = currentCenter - _previousCenter;
        float distance = displacement.magnitude;
        if (distance > 0f)
        {
            Vector3 scale = transform.lossyScale;
            float radius = _sphereCollider.radius *
                Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            RaycastHit nearestHit = default;
            bool foundHit = false;
            foreach (RaycastHit hit in Physics.SphereCastAll(_previousCenter, radius,
                         displacement / distance, distance, Physics.AllLayers, QueryTriggerInteraction.Collide))
            {
                Collider other = hit.collider;
                if (other == _sphereCollider ||
                    Physics.GetIgnoreLayerCollision(gameObject.layer, other.gameObject.layer) ||
                    Physics.GetIgnoreCollision(_sphereCollider, other))
                    continue;
                if (!foundHit || hit.distance < nearestHit.distance)
                {
                    nearestHit = hit;
                    foundHit = true;
                }
            }

            if (foundHit)
            {
                Debug.Log("Environment hit detected by sweep: " + nearestHit.collider.name);
                HandleImpact(nearestHit.collider);
            }
        }
        _previousCenter = currentCenter;
    }

    private void HandleImpact(Collider other)
    {
        if (other.CompareTag("Entity") || other.CompareTag("Player"))
        {
            EntityInfo entityInfo = other.GetComponent<EntityInfo>();
            if (entityInfo != null)
            {
                entityInfo.TakeDamage(_damage);
            }
        }

        _objectOnSphere.SetVelocity(Vector3.zero);
        _trailRenderer.emitting = false;
        _sphereCollider.enabled = false;
        _bulletPool.Return(gameObject);
    }

    private void Deactivate()
    {
        
    }

    public void Initialize(ObjectPool bulletPool, Transform firePoint, float velocity, Vector3 direction, int damage)
    {
        transform.position = firePoint.position;
        transform.rotation = firePoint.rotation;
        _damage = damage;
        _bulletPool = bulletPool;
        _objectOnSphere.SetVelocity(direction * velocity);
        _previousCenter = transform.position;
        _trailRenderer.Clear();
        _trailRenderer.emitting = true;
        _sphereCollider.enabled = true;
        _initialized = true;
    }

    private void OnDisable()
    {
        _initialized = false;
    }

}
