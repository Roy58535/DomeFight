using UnityEngine;

[RequireComponent(typeof(ObjectOnSphere))]
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(SphereCollider))]

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

    private void FixedUpdate()
    {
        // Do not test for collisions if the bullet has not been initialized yet
        if (!_initialized)
            return;

        // Perform a sweep test to detect collisions
        // Calculates displacement since last frame
        Vector3 currentCenter = transform.position;
        Vector3 displacement = currentCenter - _previousCenter;
        float distance = displacement.magnitude;

        if (distance > 0f)
        {
            // Compute true radius
            Vector3 scale = transform.lossyScale;
            float radius = _sphereCollider.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            RaycastHit nearestHit = default;
            bool foundHit = false;

            // Perform a sphere cast to detect collisions along the bullet's path
            foreach (RaycastHit hit in Physics.SphereCastAll(_previousCenter, radius,
                         displacement / distance, distance, Physics.AllLayers, QueryTriggerInteraction.Collide))
            {
                Collider other = hit.collider;
                // Ignore collisions with the bullet's own collider, layers that are set to ignore collisions, and any other colliders that should be ignored
                if (other == _sphereCollider ||
                    Physics.GetIgnoreLayerCollision(gameObject.layer, other.gameObject.layer) ||
                    Physics.GetIgnoreCollision(_sphereCollider, other))
                    continue;
                // Update the nearest hit if this hit is closer than the previous nearest hit
                if (!foundHit || hit.distance < nearestHit.distance)
                {
                    nearestHit = hit;
                    foundHit = true;
                }
            }

            if (foundHit)
            {
                // Handle the impact with the nearest hit collider if hit was found
                HandleImpact(nearestHit.collider);
            }
        }
        _previousCenter = currentCenter;
    }

    private void HandleImpact(Collider other)
    {
        if (other.CompareTag("Entity") || other.CompareTag("Player"))
        {
            // Invoke damage on entity (temporary test functionality)
            EntityInfo entityInfo = other.GetComponent<EntityInfo>();
            if (entityInfo != null)
            {
                entityInfo.TakeDamage(_damage);
            }
        }

        // Deactivate the bullet and return it to the pool once it has hit anything
        _objectOnSphere.SetVelocity(Vector3.zero);
        _trailRenderer.emitting = false;
        _sphereCollider.enabled = false;
        _bulletPool.Return(gameObject);
    }

    public void Initialize(ObjectPool bulletPool, Transform firePoint, float velocity, Vector3 direction, int damage)
    {
        // Initialize all runtime values for the bullet
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
