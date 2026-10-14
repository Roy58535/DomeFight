using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weapons;
    [SerializeField] private int _currWeaponIdx;
    [SerializeField] private int _defaultWeaponIdx;
    [SerializeField] private float _fireBuffer = 0.15f; // Buffer time for semi-auto firing

    //private const float TRIGGER_THRESHOLD = 0.5f; // Threshold for trigger input
    //private float _prevRightTrigger = 0f; // Previous frame's right trigger 
    private float _lastRequestedFireTime = -Mathf.Infinity; // Time of the last fire request
    private PlayerControls _playerControls;
    private AimController _aimController;

    private void Start()
    {
        _playerControls = GetComponent<PlayerControls>();
        _aimController = GetComponent<AimController>();
        // Switch to default weapon at start
        SwitchToWeapon(_defaultWeaponIdx);
    }

    private void Update()
    {
        _lastRequestedFireTime = _playerControls.FirePressed ? Time.time : _lastRequestedFireTime;
        

        if (_weapons != null && _weapons.Count > 0)
        {
            Weapon weapon = _weapons[_currWeaponIdx];
            if (weapon != null)
            {
                if (weapon.IsAutomatic && _playerControls.FireHeld)
                {
                    weapon.Fire(_aimController.AimDirection);
                }
                else if (Time.time - _lastRequestedFireTime <= _fireBuffer && weapon.CanFire)
                {
                    print(weapon.name + " fired at " + Time.time);
                    weapon.Fire(_aimController.AimDirection);
                    _lastRequestedFireTime = -Mathf.Infinity; // Reset after firing
                }

                if (_playerControls != null && _playerControls.ReloadPressed)
                {
                    weapon.Reload();
                }
            }
        }

        // Cycle weapons with Left Bumper (LB)
        if (_playerControls != null && _playerControls.CycleWeaponPressed)
        {
            if (_weapons != null && _weapons.Count > 0)
            {
                int nextIndex = (_currWeaponIdx + 1) % _weapons.Count;
                SwitchToWeapon(nextIndex);
            }
        }
    }

    private void SwitchToWeapon(int index)
    {
        // Switch to the weapon at the specified index
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
