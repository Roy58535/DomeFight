using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(SphereCollider))]
public class Bullet : MonoBehaviour
{
    // Flight duration, curved-path resolution, and optional tangent gravity.
    [SerializeField, Min(0.01f)] private float _maxLifetime = 5f;
    [SerializeField, Min(0.01f)] private float _maxSegmentLength = 0.2f;
    [SerializeField] private bool _useGravity = true;
    [SerializeField, Min(0f)] private float _gravity = 9f;

    // Layer and trigger filtering for physics queries.
    [SerializeField] private LayerMask _hitMask = ~0;
    [SerializeField] private QueryTriggerInteraction _triggerInteraction = QueryTriggerInteraction.Collide;

    // Cached presentation, collision-shape, and pool references.
    private TrailRenderer _trail;
    private SphereCollider _sphereCollider;
    private ObjectPool _pool;

    // Position and velocity, maintained on the arena sphere.
    private Vector3 _position;
    private Vector3 _velocity;
    private float _sphereRadius;
    private float _collisionRadius;
    private float _remainingLifetime;
    private int _damage;

    // Launch clearance references.
    private EntityInfo _firingEntity;
    private Collider[] _launchColliders;
    private bool _hasClearedShooter;

    // Active-shot state and reusable physics-query buffers.
    private bool _flying;
    private Collider[] _overlaps = new Collider[16];
    private RaycastHit[] _hits = new RaycastHit[16];

    private void Awake()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        // Cache the trail renderer and sphere collider references.
        if (_trail == null)
            _trail = GetComponent<TrailRenderer>();

        if (_sphereCollider == null)
            _sphereCollider = GetComponent<SphereCollider>();

        // Disable collider contacts and automatic trail emission.
        _sphereCollider.enabled = false;
        _trail.emitting = false;
    }

    private void OnEnable()
    {
        CacheComponents();

        // Reset flight state and clear the trail when the bullet is activated.
        _flying = false;
        _trail.Clear();
    }

    private void OnDisable()
    {
        // Stop flight, reset velocity, and clear the trail.
        _flying = false;
        _velocity = Vector3.zero;

        if (_trail != null)
        {
            _trail.emitting = false;
            _trail.Clear();
        }
    }

    public void Initialize(ObjectPool bulletPool, Transform firePoint, float velocity, Vector3 direction, int damage)
    {
        // Set the pool and arena radius, then validate the launch parameters.
        CacheComponents();
        _pool = bulletPool;
        _flying = false;
        _sphereRadius = ObjectOnSphere.Radius;

        if (_pool == null || firePoint == null || firePoint.position.sqrMagnitude < 0.000001f || _sphereRadius <= 0f)
        {
            Debug.LogError("Bullet requires a pool, a nonzero muzzle position, and a positive arena radius.", this);

            if (_pool != null)
                _pool.Return(gameObject);
            else
                gameObject.SetActive(false);

            return;
        }

        // Snap the muzzle to the sphere and convert the shot direction into tangent velocity.
        _position = firePoint.position.normalized * _sphereRadius;
        Vector3 tangent = Vector3.ProjectOnPlane(direction, _position.normalized);
        _velocity = tangent.normalized * Mathf.Max(0f, velocity);
        _damage = damage;

        // Collect shooter colliders and reset launch clearance.
        _firingEntity = firePoint.GetComponentInParent<EntityInfo>();
        _launchColliders = _firingEntity != null
            ? _firingEntity.GetComponentsInChildren<Collider>(true)
            : System.Array.Empty<Collider>();
        _hasClearedShooter = false;

        // Reset lifetime and derive a world-space sweep radius from the collider scale.
        _remainingLifetime = Mathf.Max(0.01f, _maxLifetime);
        Vector3 scale = transform.lossyScale;
        _collisionRadius = Mathf.Max(0.0001f, _sphereCollider.radius *
            Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));

        // Place the bullet at the muzzle, clear the trail, and enable emission.
        transform.SetPositionAndRotation(_position, firePoint.rotation);
        _trail.Clear();
        _trail.AddPosition(_position);
        _trail.enabled = true;
        _trail.emitting = true;

        // Begin hit detection immediately, including objects overlapping the launch position.
        _flying = true;
        UpdateLaunchClearance();
        CheckOverlap();
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        if (!_flying || deltaTime <= 0f)
            return;

        UpdateLaunchClearance();

        // Check for colliders overlapping the current bullet position.
        if (CheckOverlap())
            return;

        // Divide the remaining flight time into short curved-path segments.
        float duration = Mathf.Min(deltaTime, _remainingLifetime);
        float acceleration = _useGravity ? Mathf.Max(0f, _gravity) : 0f;
        float maxDistance = Mathf.Max(0.01f, Mathf.Min(_maxSegmentLength, _sphereRadius * 2f * Mathf.Deg2Rad));
        int segments = Mathf.Max(1, Mathf.CeilToInt((_velocity.magnitude + acceleration * duration) * duration / maxDistance));
        float step = duration / segments;

        for (int i = 0; i < segments && _flying; i++)
        {
            // Apply gravity along the sphere's downward tangent before advancing this substep.
            Vector3 normal = _position / _sphereRadius;
            Vector3 down = Vector3.ProjectOnPlane(Vector3.down, normal);

            if (down.sqrMagnitude > 0.000001f)
                _velocity += down.normalized * acceleration * step;

            float speed = _velocity.magnitude;
            if (speed > 0.000001f)
            {
                // Calculate the next position by rotating along the sphere.
                Vector3 axis = Vector3.Cross(normal, _velocity / speed).normalized;
                Quaternion turn = Quaternion.AngleAxis(speed * step / _sphereRadius * Mathf.Rad2Deg, axis);
                Vector3 next = (turn * normal).normalized * _sphereRadius;
                Vector3 displacement = next - _position;

                // Sweep to the next position and handle the first valid impact.
                RaycastHit hit;
                if (TrySweep(displacement, out hit))
                {
                    CommitPosition(_position + displacement.normalized * hit.distance);
                    Impact(hit.collider);
                    return;
                }

                // Rotate velocity, move the bullet, and update launch clearance.
                _velocity = turn * _velocity;
                CommitPosition(next);
                UpdateLaunchClearance();
            }
        }

        // Reduce remaining lifetime and release expired bullets.
        _remainingLifetime -= duration;
        if (_flying && _remainingLifetime <= 0f)
        {
            _flying = false;
            Release();
        }
    }

    private void CommitPosition(Vector3 position)
    {
        _position = position;
        transform.position = position;

        // Add the new position to the trail.
        _trail.AddPosition(position);
    }

    private void UpdateLaunchClearance()
    {
        if (_hasClearedShooter)
            return;

        // Check whether the bullet sphere still overlaps any shooter collider.
        float clearance = _collisionRadius + 0.001f;

        foreach (Collider collider in _launchColliders)
        {
            if (collider != null && collider.enabled && collider.gameObject.activeInHierarchy &&
                (collider.ClosestPoint(_position) - _position).sqrMagnitude <= clearance * clearance)
                return;
        }

        // Enable self-collision permanently after clearing the shooter.
        _hasClearedShooter = true;
    }

    private bool IsValidTarget(Collider other)
    {
        // Exclude projectile colliders, temporary launch protection, and explicitly ignored pairs.
        return other != null && other != _sphereCollider &&
            other.GetComponentInParent<Bullet>() == null &&
            (_hasClearedShooter || _firingEntity == null || other.GetComponentInParent<EntityInfo>() != _firingEntity) &&
            !Physics.GetIgnoreLayerCollision(gameObject.layer, other.gameObject.layer) &&
            !Physics.GetIgnoreCollision(_sphereCollider, other);
    }

    private bool CheckOverlap()
    {
        // Collect overlapping colliders, expanding the buffer when it fills.
        int count;
        while ((count = Physics.OverlapSphereNonAlloc(_position, _collisionRadius, _overlaps,
                   _hitMask, _triggerInteraction)) == _overlaps.Length)
            System.Array.Resize(ref _overlaps, _overlaps.Length * 2);

        // Select the closest valid collider when the bullet already overlaps several objects.
        Collider nearest = null;
        float nearestDistance = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            Collider other = _overlaps[i];
            if (!IsValidTarget(other))
                continue;

            float distance = (other.ClosestPoint(_position) - _position).sqrMagnitude;
            if (distance < nearestDistance)
            {
                nearest = other;
                nearestDistance = distance;
            }
        }

        if (nearest == null)
            return false;

        // Apply the overlapping collider impact.
        Impact(nearest);
        return true;
    }

    private bool TrySweep(Vector3 displacement, out RaycastHit nearest)
    {
        // Skip the sweep when there is no displacement.
        nearest = default;
        float distance = displacement.magnitude;
        if (distance <= 0f)
            return false;

        // Collect swept contacts, expanding the buffer when it fills.
        int count;
        while ((count = Physics.SphereCastNonAlloc(_position, _collisionRadius, displacement / distance,
                   _hits, distance, _hitMask, _triggerInteraction)) == _hits.Length)
            System.Array.Resize(ref _hits, _hits.Length * 2);

        // Select the closest valid swept contact.
        bool found = false;

        for (int i = 0; i < count; i++)
        {
            if (!IsValidTarget(_hits[i].collider))
                continue;

            if (!found || _hits[i].distance < nearest.distance)
            {
                nearest = _hits[i];
                found = true;
            }
        }

        return found;
    }

    private void Impact(Collider other)
    {
        if (!_flying)
            return;

        // Stop flight before applying impact damage.
        _flying = false;

        try
        {
            // Child hitboxes resolve damage against their owning entity.
            EntityInfo entity = other.GetComponentInParent<EntityInfo>();
            if (entity != null)
                entity.TakeDamage(_damage);
        }
        finally
        {
            // Return the bullet to the pool after damage handling.
            Release();
        }
    }

    private void Release()
    {
        // Reset velocity, stop emission, and clear the trail before returning to the pool.
        _velocity = Vector3.zero;
        _trail.emitting = false;
        _trail.Clear();

        _pool.Return(gameObject);
    }
}
