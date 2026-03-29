using UnityEngine;
using UnityEngine.AI;

public class Weapon : MonoBehaviour
{
    public float Damage;
    public int CurrentAmmo;
    public int MaxAmmo; 
    public float FireRate; // RPS
    public bool CanFire;

    [SerializeField] private Transform _firePoint;

    private bool _isReloading;
    private float _lastFireTime;

    private void Start()
    {
        
    }

    private void Update()
    {

        CanFire = !_isReloading && CurrentAmmo > 0 && Time.time - _lastFireTime >= 1 / FireRate;
    }

    public void Fire()
    {
        if (CanFire)
        {
            CurrentAmmo--;
            _lastFireTime = Time.time;
        }
    }
}