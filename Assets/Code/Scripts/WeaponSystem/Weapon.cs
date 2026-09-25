using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int Damage;
    public int CurrentAmmo;
    public int MaxAmmo;
    public float BulletSpeed;
    public float BulletSize;
    public float FireRate; // RPS
    public bool CanFire;

    [SerializeField] private Transform _firePoint;
    [SerializeField] private ObjectPool _bulletPool;

    private bool _isReloading;
    private float _lastFireTime;

    private void Update()
    {
        // Update firing state based on ammo, reload status, and fire rate
        CanFire = !_isReloading && CurrentAmmo > 0 && Time.time - _lastFireTime >= 1 / FireRate;
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
}
