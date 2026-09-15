using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    private TrailRenderer _trailRenderer;
    private ObjectOnSphere _objectOnSphere;
    private Collider _collider;
    private SphereCollider _sphereCollider;
    private Vector3 _previousCenter;
    private bool _sweepInitialized;

    private void Awake()
    {
        _objectOnSphere = GetComponent<ObjectOnSphere>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _collider = GetComponent<Collider>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        Initialize(0, 0f, Vector3.zero);
    }

    private void FixedUpdate()
    {
        // Sweep test for environment collision
        if (!_sweepInitialized || !_collider.enabled || _sphereCollider == null)
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
                if (other == _collider ||
                    Physics.GetIgnoreLayerCollision(gameObject.layer, other.gameObject.layer) ||
                    Physics.GetIgnoreCollision(_collider, other))
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
                entityInfo.TakeDamage(damage);
            }
        }
        gameObject.SetActive(false);
        _trailRenderer.emitting = false;
        _collider.enabled = false;
        //Invoke("Deactivate", _trailRenderer.time);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Initialize(int damage, float velocity, Vector3 direction)
    {
        this.damage = damage;
        _objectOnSphere.SetVelocity(direction * velocity);
        if (_sphereCollider != null)
        {
            _previousCenter = transform.position;
            _sweepInitialized = true;
        }
    }

    private void OnDisable()
    {
        _sweepInitialized = false;
    }

}
