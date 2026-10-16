using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerControls : MonoBehaviour
{
    private InputAction _move;
    private InputAction _jump;
    private InputAction _aim;
    private InputAction _fire;
    private InputAction _reload;
    private InputAction _cycleWeapon;

    private void Start()
    {
        InputActionAsset actions = GetComponent<PlayerInput>().actions;

        _move = actions.FindAction("Gameplay/Move", true);
        _jump = actions.FindAction("Gameplay/Jump", true);
        _aim = actions.FindAction("Gameplay/Aim", true);
        _fire = actions.FindAction("Gameplay/Fire", true);
        _reload = actions.FindAction("Gameplay/Reload", true);
        _cycleWeapon = actions.FindAction("Gameplay/CycleWeapon", true);
    }

    public Vector2 Move => _move?.ReadValue<Vector2>() ?? Vector2.zero;

    public Vector2 Aim => _aim?.ReadValue<Vector2>() ?? Vector2.zero;

    public bool JumpPressed => _jump?.WasPressedThisFrame() ?? false;

    public bool FirePressed => _fire?.WasPressedThisFrame() ?? false;

    public bool FireHeld => _fire?.IsPressed() ?? false;

    public bool ReloadPressed => _reload?.WasPressedThisFrame() ?? false;

    public bool CycleWeaponPressed => _cycleWeapon?.WasPressedThisFrame() ?? false;
}
