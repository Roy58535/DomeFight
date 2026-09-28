using UnityEngine;
using UnityEngine.InputSystem;

public class AimController : MonoBehaviour
{
    public Vector3 AimDirection { get; private set; }

    [SerializeField] private Transform _eyeTransform;
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private Transform _weaponAnchorTransform;
    [SerializeField] private float _eyeRotationRadius = 0.15f;

    private ObjectOnSphere _objectOnSphere;
    

    private void Awake()
    {
        _objectOnSphere = GetComponent<ObjectOnSphere>();
        AimDirection = -_objectOnSphere.AzimuthDir;
    }

    private void Update()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null)
        {
            Vector2 rightStick = gamepad.rightStick.ReadValue();
            Vector2 rightStickDir = rightStick.normalized;
            float zDir = rightStickDir.y * -90f;
            float yDir = rightStickDir.x > 0f ? 180f : 0f;

            if (rightStick.sqrMagnitude > 0.01f)
            {
                // Calculate the aim direction based on the right stick input
                AimDirection = -rightStickDir.x * _objectOnSphere.AzimuthDir + rightStickDir.y * -_objectOnSphere.SurfaceDownDir;

                // Rotate the eye around the body based on the aim direction
                _eyeTransform.localPosition = new Vector3(rightStickDir.x * _eyeRotationRadius, rightStickDir.y * _eyeRotationRadius, _eyeTransform.localPosition.z);

                _weaponAnchorTransform.localRotation = Quaternion.Euler(0, yDir, zDir);
            }
        }
    }
}
