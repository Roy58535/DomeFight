using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weapons;
    [SerializeField] private int _currWeaponIdx;
    [SerializeField] private int _defaultWeaponIdx;
    [SerializeField] private AimController _aimController;

    private float _shootingAngle;
    private ObjectOnSphere _objectOnSphere;

    private void Start()
    {
        _objectOnSphere = GetComponent<PlayerMovement>();
        // Switch to default weapon at start
        SwitchToWeapon(_defaultWeaponIdx);
    }

    private void Update()
    {
        Gamepad gamepad = Gamepad.current;
        float rt = 0f;
        Vector2 rightStick = Vector2.zero;
        Vector2 leftStick = Vector2.zero;

        if (gamepad != null)
        {
            rt = gamepad.rightTrigger.ReadValue();
            rightStick = gamepad.rightStick.ReadValue();
            leftStick = gamepad.leftStick.ReadValue();
        }

        rightStick.Normalize();
        leftStick.Normalize();

        _shootingAngle = Mathf.Atan2(rightStick.y, rightStick.x);

        if (rt > 0.5f)
        {
            if (_weapons != null && _weapons.Count > 0)
            {
                Weapon weapon = _weapons[_currWeaponIdx];
                if (weapon != null)
                {
                    // Calculate the firing direction based on right stick angle
                    Vector3 fireDir = -rightStick.x * _objectOnSphere.AzimuthDir + rightStick.y * -_objectOnSphere.SurfaceDownDir;
                    weapon.Fire(_aimController.AimDirection);
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
