using UnityEngine;
using UnityEngine.InputSystem;

public class AimController : MonoBehaviour
{
    // public Vector3 AimDirection { get; private set; }

    [SerializeField] private Transform _eyeTransform;
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private Transform _weaponAnchorTransform;
    [SerializeField] private float _eyeRotationRadius = 0.15f;
    //[SerializeField] private float _baseEyeHeight;
    [SerializeField, Range(-90f, 0f)] private float _minEyeAngle = -20f;
    [SerializeField, Range(0f, 90f)] private float _maxEyeAngle = 35f;

    [SerializeField, Min(0.01f)] private float _aimSharpness = 20f;
    [SerializeField, Range(0.01f, 1f)] private float _facingThreshold = 0.15f;

    // private ObjectOnSphere _objectOnSphere;
    private PlayerControls _playerControls;
    private float _baseEyeHeight;
    private float _currentAimAngle;
    private float _targetAimAngle;
    private bool _facingRight;


    private void Awake()
    {
        // _objectOnSphere = GetComponent<ObjectOnSphere>();
        _playerControls = GetComponent<PlayerControls>();

        // Store the eyes' authored height above the body's center.
        _baseEyeHeight = _eyeTransform.localPosition.y;
        // AimDirection = -_objectOnSphere.AzimuthDir;

        // Start smoothing from the weapon's authored rotation.
        Vector3 weaponAngles = _weaponAnchorTransform.localEulerAngles;
        _currentAimAngle = Mathf.DeltaAngle(0f, weaponAngles.z);
        _targetAimAngle = _currentAimAngle;
        _facingRight = Mathf.Abs(Mathf.DeltaAngle(weaponAngles.y, 180f)) < 90f;
    }

    private void Update()
    {
        if (_playerControls != null)
        {
            Vector2 rightStick = _playerControls.Aim;
            Vector2 rightStickDir = rightStick.normalized;

            if (rightStick.sqrMagnitude > 0.01f)
            {
                // Calculate the aim direction based on the right stick input
                // AimDirection = -rightStickDir.x * _objectOnSphere.AzimuthDir + rightStickDir.y * -_objectOnSphere.SurfaceDownDir;

                // Clamp eye elevation relative to the left or right horizontal direction.
                float eyeAngle = Mathf.Atan2(rightStickDir.y, Mathf.Abs(rightStickDir.x)) * Mathf.Rad2Deg;
                _targetAimAngle = -eyeAngle;

                // Retain facing near vertical input to prevent repeated side flips.
                if (rightStickDir.x > _facingThreshold)
                    _facingRight = true;
                else if (rightStickDir.x < -_facingThreshold)
                    _facingRight = false;

                float clampedEyeAngle = Mathf.Clamp(eyeAngle, _minEyeAngle, _maxEyeAngle) * Mathf.Deg2Rad;
                float eyeSide = _facingRight ? 1f : -1f;

                // Move the eyes along the limited arc above their authored height.
                _eyeTransform.localPosition = new Vector3(
                    eyeSide * Mathf.Cos(clampedEyeAngle) * _eyeRotationRadius,
                    _baseEyeHeight + Mathf.Sin(clampedEyeAngle) * _eyeRotationRadius,
                    _eyeTransform.localPosition.z);

            }

            // Smooth elevation toward the last aim input independently of frame rate.
            float blend = 1f - Mathf.Exp(-_aimSharpness * Time.deltaTime);
            _currentAimAngle = Mathf.Lerp(_currentAimAngle, _targetAimAngle, blend);

            // Apply the side flip immediately while keeping elevation smooth.
            _weaponAnchorTransform.localRotation = Quaternion.Euler(
                0f, _facingRight ? 180f : 0f, _currentAimAngle);
        }
    }
}
