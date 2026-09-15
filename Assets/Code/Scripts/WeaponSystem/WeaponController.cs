using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weapons;
    [SerializeField] private int _currWeaponIdx;
    [SerializeField] private int _defaultWeaponIdx;
    private ObjectOnSphere _objectOnSphere;
    [SerializeField] private float _shootingAngle = 1.0f;

    private void Start()
    {
        _objectOnSphere = GetComponent<PlayerMovement>();
        SwitchToWeapon(_defaultWeaponIdx);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_weapons != null && _weapons.Count > 0)
            {
                Weapon weapon = _weapons[_currWeaponIdx];
                if (weapon != null)
                {

                    Vector3 fireDir = Mathf.Cos(_shootingAngle) * _objectOnSphere.AzimuthDir + Mathf.Sin(_shootingAngle) * -_objectOnSphere.SurfaceDownDir;
                    weapon.Fire(fireDir);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchToWeapon(2);
        }
    }

    private void SwitchToWeapon(int index)
    {
        if (index >= 0 && index < _weapons.Count)
        {
            _currWeaponIdx = index;
            for (int i = 0; i < _weapons.Count; i++)
            {
                _weapons[i].gameObject.SetActive(i == _currWeaponIdx);
            }
        }
    }
}
