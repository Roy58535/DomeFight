using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weapons;
    [SerializeField] private int _currWeaponIdx;
    [SerializeField] private int _defaultWeaponIdx;

    private void Start()
    {
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
                    weapon.Fire();
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
