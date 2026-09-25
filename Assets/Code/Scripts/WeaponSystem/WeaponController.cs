using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<Weapon> _weapons;
    [SerializeField] private int _currWeaponIdx;
    [SerializeField] private int _defaultWeaponIdx;
    
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
        float rt = gamepad.rightTrigger.ReadValue();
        Vector2 rightStick = gamepad.rightStick.ReadValue();
        Vector2 leftStick = gamepad.leftStick.ReadValue();
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
