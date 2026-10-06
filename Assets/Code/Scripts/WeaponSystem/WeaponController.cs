using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weapons;
    [SerializeField] private int _currWeaponIdx;
    [SerializeField] private int _defaultWeaponIdx;
    [SerializeField] private AimController _aimController;
    [SerializeField] private float _fireBuffer = 0.15f; // Buffer time for semi-auto firing

    private const float TRIGGER_THRESHOLD = 0.5f; // Threshold for trigger input
    private float _prevRightTrigger = 0f; // Previous frame's right trigger 
    private float _lastRequestedFireTime = -Mathf.Infinity; // Time of the last fire request

    private void Start()
    {
        // Switch to default weapon at start
        SwitchToWeapon(_defaultWeaponIdx);
    }

    private void Update()
    {
        Gamepad gamepad = Gamepad.current;
        float rt = 0f;

        if (gamepad != null)
        {
            rt = gamepad.rightTrigger.ReadValue();
            
        }

        bool rightTriggerPressedThisFrame = rt > TRIGGER_THRESHOLD && _prevRightTrigger <= TRIGGER_THRESHOLD;
        _lastRequestedFireTime = rightTriggerPressedThisFrame ? Time.time : _lastRequestedFireTime;
        

        if (_weapons != null && _weapons.Count > 0)
        {
            Weapon weapon = _weapons[_currWeaponIdx];
            if (weapon != null)
            {
                if (weapon.IsAutomatic && rt > TRIGGER_THRESHOLD)
                {
                    weapon.Fire(_aimController.AimDirection);
                }
                else if (Time.time - _lastRequestedFireTime <= _fireBuffer && weapon.CanFire)
                {
                    weapon.Fire(_aimController.AimDirection);
                    _lastRequestedFireTime = -Mathf.Infinity; // Reset after firing
                }

                if (gamepad != null && gamepad.rightShoulder.wasPressedThisFrame)
                {
                    weapon.Reload();
                }
            }
        }

        // Cycle weapons with Left Bumper (LB)
        if (gamepad != null && gamepad.leftShoulder.wasPressedThisFrame)
        {
            if (_weapons != null && _weapons.Count > 0)
            {
                int nextIndex = (_currWeaponIdx + 1) % _weapons.Count;
                SwitchToWeapon(nextIndex);
            }
        }

        _prevRightTrigger = rt; // Update previous trigger value for next frame
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
