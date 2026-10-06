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
    [SerializeField] private ObjectPool _bulletPool;


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

    public void Fire(Vector3 dir)
    {
        if (CanFire)
        {
            CurrentAmmo--;
            _lastFireTime = Time.time;

            // Get a bullet from the pool and initialize it
            GameObject bullet = _bulletPool.Get();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.Initialize(_bulletPool, _firePoint, BulletSpeed, dir, Damage);
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
