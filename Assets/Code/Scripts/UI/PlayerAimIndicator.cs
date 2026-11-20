using UnityEngine;

[RequireComponent(typeof(WeaponController))]
public class PlayerAimIndicator : MonoBehaviour
{
    [SerializeField] private LineRenderer _line;
    [SerializeField, Min(0.01f)] private float _length = 3f;

    private WeaponController _weapons;
    private EntityInfo _shooter;
    private Collider[] _shooterColliders;
    private readonly Vector3[] _points = new Vector3[1024];
    private RaycastHit[] _hits = new RaycastHit[16];
    private Collider[] _overlaps = new Collider[16];

    private void Awake()
    {
        // Cache the player's launch-clearance colliders and weapon selection.
        _weapons = GetComponent<WeaponController>();
        _shooter = GetComponent<EntityInfo>();
        _shooterColliders = GetComponentsInChildren<Collider>(true);
        _line.enabled = false;
    }

    private void OnDisable()
    {
        _line.enabled = false;
    }

    private void LateUpdate()
    {
        // Read the active muzzle after aiming and weapon switching have updated.
        Weapon weapon = _weapons.CurrentWeapon;
        Bullet template = weapon != null ? weapon.ProjectileTemplate : null;
        if (template == null || !weapon.isActiveAndEnabled || ObjectOnSphere.Radius <= 0f)
        {
            _line.enabled = false;
            return;
        }

        Transform muzzle = weapon.FirePoint;
        if (muzzle == null || muzzle.position.sqrMagnitude < 0.000001f)
        {
            _line.enabled = false;
            return;
        }

        // Match the projected launch position and center direction of a real shot.
        float radius = ObjectOnSphere.Radius;
        Vector3 position = muzzle.position.normalized * radius;
        Vector3 velocity = Vector3.ProjectOnPlane(muzzle.forward, position.normalized).normalized * Mathf.Max(0f, weapon.BulletSpeed);
        if (velocity.sqrMagnitude < 0.000001f)
        {
            _line.enabled = false;
            return;
        }

        // Predict complete physics steps using the bullet's gravity and subdivision settings.
        int count = 1;
        _points[0] = position;
        float distance = 0f;
        float remaining = template.Lifetime;
        float collisionRadius = template.CollisionRadius;
        bool cleared = false;
        bool stopped = false;

        while (remaining > 0f && count < _points.Length && !stopped)
        {
            // Match the overlap check at the beginning of each projectile physics step.
            cleared = cleared || HasClearedShooter(position, collisionRadius);
            if (IsOverlapping(template, position, collisionRadius, cleared))
                break;

            float duration = Mathf.Min(Time.fixedDeltaTime, remaining);
            if (duration <= 0f)
                break;

            int segments = BulletTrajectory.SegmentCount(velocity, template.Acceleration, duration, radius, template.SegmentLength);
            float step = duration / segments;

            for (int i = 0; i < segments && count < _points.Length; i++)
            {
                // Ignore the shooter only until the projectile sphere clears its colliders.
                cleared = cleared || HasClearedShooter(position, collisionRadius);
                Vector3 next = BulletTrajectory.Advance(position, ref velocity, radius, template.Acceleration, step);
                Vector3 displacement = next - position;
                float segmentDistance = displacement.magnitude;
                if (segmentDistance <= 0.000001f)
                {
                    stopped = true;
                    break;
                }

                // Clip to the first hit or the configured finite preview distance.
                float travel = Mathf.Min(segmentDistance, _length - distance);
                float hitDistance;
                bool hit = Sweep(template, position, collisionRadius, displacement / segmentDistance, travel, cleared, out hitDistance);
                if (hit)
                    travel = hitDistance;

                _points[count++] = position + displacement / segmentDistance * travel;
                distance += travel;
                position = next;
                if (hit || distance >= _length)
                {
                    stopped = true;
                    break;
                }
            }

            remaining -= duration;
        }

        // Keep unused buffer entries out of the rendered line.
        _line.positionCount = count;
        for (int i = 0; i < count; i++)
            _line.SetPosition(i, _points[i]);
        _line.enabled = count > 1;
    }

    private bool HasClearedShooter(Vector3 position, float radius)
    {
        float clearance = radius + 0.001f;
        foreach (Collider collider in _shooterColliders)
        {
            if (collider != null && collider.enabled && collider.gameObject.activeInHierarchy &&
                (collider.ClosestPoint(position) - position).sqrMagnitude <= clearance * clearance)
                return false;
        }

        return true;
    }

    private bool IsOverlapping(Bullet template, Vector3 position, float radius, bool cleared)
    {
        // Use the same sphere shape, masks, and target filters as projectile overlaps.
        int count;
        while ((count = Physics.OverlapSphereNonAlloc(position, radius, _overlaps, template.HitMask, template.TriggerInteraction)) == _overlaps.Length)
            System.Array.Resize(ref _overlaps, _overlaps.Length * 2);

        for (int i = 0; i < count; i++)
        {
            if (template.IsPredictionTarget(_overlaps[i], _shooter, cleared))
                return true;
        }

        return false;
    }

    private bool Sweep(Bullet template, Vector3 position, float radius, Vector3 direction, float distance, bool cleared, out float nearest)
    {
        // Select the closest valid contact along this curved-path segment.
        int count;
        while ((count = Physics.SphereCastNonAlloc(position, radius, direction, _hits, distance, template.HitMask, template.TriggerInteraction)) == _hits.Length)
            System.Array.Resize(ref _hits, _hits.Length * 2);

        nearest = float.PositiveInfinity;
        for (int i = 0; i < count; i++)
        {
            if (template.IsPredictionTarget(_hits[i].collider, _shooter, cleared))
                nearest = Mathf.Min(nearest, _hits[i].distance);
        }

        return nearest < float.PositiveInfinity;
    }
}
