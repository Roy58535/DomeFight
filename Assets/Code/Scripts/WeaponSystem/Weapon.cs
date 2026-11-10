using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int Damage;
    public int CurrentAmmo;
    public int MaxAmmo;
    public float BulletSpeed;
    public float BulletSize;
    public float FireRate; // RPS
    public float ReloadTime;
    public bool IsAutomatic;

    public bool CanFire => _canFire;
    public bool CanReload => _canReload;

    [SerializeField] private Transform _firePoint;

    // Number of projectiles per shot and angular spread within the sphere's tangent plane.
    [SerializeField, Min(1)] private int _pelletsPerShot = 1;
    [SerializeField, Range(0f, 90f)] private float _spreadHalfAngle = 0f;

    private ObjectPool _bulletPool;

    private bool _canFire;
    private bool _canReload;
    private bool _isReloading;
    private float _lastFireTime;
    private float _reloadTimer;

    private void Update()
    {
        if (_isReloading)
        {
            _reloadTimer -= Time.deltaTime;
            if (_reloadTimer <= 0)
            {
                _isReloading = false;
                CurrentAmmo = MaxAmmo;
            }
        }

        // Update firing state based on ammo, reload status, and fire rate
        _canFire = !_isReloading && CurrentAmmo > 0 && Time.time - _lastFireTime >= 1 / FireRate;
        _canReload = !_isReloading && CurrentAmmo < MaxAmmo;
    }
    
    public void Initialize(ObjectPool bulletPool)
    {
        _bulletPool = bulletPool;
    }

    public void Fire()
    {
        // Validate the shot and project the barrel direction onto the muzzle's tangent plane.
        if (!CanFire)
            return;

        Vector3 normal = _firePoint.position.normalized;
        Vector3 forward = Vector3.ProjectOnPlane(_firePoint.forward, normal).normalized;
        if (forward.sqrMagnitude < 0.000001f)
            return;

        // Consume one round and start one cooldown for the complete pellet group.
        CurrentAmmo--;
        _lastFireTime = Time.time;
        _canFire = false;

        int pelletCount = Mathf.Max(1, _pelletsPerShot);
        float halfAngle = Mathf.Clamp(_spreadHalfAngle, 0f, 90f);

        for (int i = 0; i < pelletCount; i++)
        {
            // Place one pellet randomly in the spread.
            float angle = halfAngle > 0f
                ? Random.Range(-halfAngle, halfAngle)
                : 0f;
            Vector3 direction = Quaternion.AngleAxis(angle, normal) * forward;

            // Launch each pellet through the existing bullet lifecycle and pool.
            GameObject bullet = _bulletPool.Get();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.Initialize(_bulletPool, _firePoint, BulletSpeed, direction, Damage);
            }
        }
    }

    public void Reload()
    {
        if (_canReload)
        {
            _isReloading = true;
            _reloadTimer = ReloadTime;
        }
    }

    private void OnDisable()
    {
        _isReloading = false;
        _reloadTimer = 0f;
    }
}
