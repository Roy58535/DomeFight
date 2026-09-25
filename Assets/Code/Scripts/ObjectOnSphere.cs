using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectOnSphere : MonoBehaviour
{
    public static float GravityStrength = 9f;
    public static float Radius = 10f;

    [SerializeField] private bool _useGravity = false;
    [SerializeField] private Vector3 _sphericalCoord;
    [SerializeField] private Vector3 _initialCoord;
    [SerializeField] private bool _useInitialCoord = false;
    public Vector3 SphericalCoord => _sphericalCoord;
    public Vector3 AzimuthDir => _azimuthDir;
    public Vector3 RadialDir => _radialDir;
    public Vector3 SurfaceDownDir => _surfaceDownDir;

    private Rigidbody _rig;
    protected Rigidbody Rig => _rig;

    private Vector3 _azimuthDir;
    private Vector3 _radialDir;
    private Vector3 _surfaceDownDir;

    protected virtual void Awake()
    {
        _rig = GetComponent<Rigidbody>();

        // Calculate initial spherical coordinates based on the initial position or use the provided initial coordinates
        if (_useInitialCoord)
        {
            _sphericalCoord = _initialCoord;
        }
        else
        {
            _sphericalCoord = SphericalCoordinatesUtils.CartesianToSpherical(transform.position);
        }

        // Set the initial position based on the spherical coordinates
        SetSphericalPosition(_sphericalCoord);
    }

    protected virtual void Update()
    {
        // Constrain orientation
        transform.forward = transform.position;
    }

    protected virtual void FixedUpdate()
    {
        // Constrain position and velocity to sphere
        ConstrainToSphere();
        // Update spherical coordinates based on the current position
        _sphericalCoord = SphericalCoordinatesUtils.CartesianToSpherical(transform.position);
        
        // Compute radial, azimuthal and surfacedown directions
        _azimuthDir = new Vector3(-Mathf.Sin(_sphericalCoord.y), 0, Mathf.Cos(_sphericalCoord.y)).normalized;
        _radialDir = transform.position.normalized;
        _surfaceDownDir = Vector3.ProjectOnPlane(Vector3.down, _radialDir).normalized;

        // Ensure minimum gravity near pole
        if (_surfaceDownDir.magnitude < 1e-6f)
        {
            _surfaceDownDir = Vector3.Cross(_azimuthDir, _radialDir).normalized;
        }

        // Apply gravity force
        if (_useGravity)
        {
            Rig.AddForce(_surfaceDownDir * GravityStrength, ForceMode.Acceleration);
        }
    }
    
    protected void ConstrainToSphere()
    {
        // Constrain position to sphere
        Vector3 dir = transform.position.normalized;
        transform.position = dir * Radius;
        // Constrain velocity to sphere
        Rig.velocity = Vector3.ProjectOnPlane(Rig.velocity, Rig.position.normalized);
    }

    public void SetSphericalPosition(Vector3 sphericalCoord)
    {
        // Set the position based on spherical coordinates
        _sphericalCoord = sphericalCoord;
        transform.position = SphericalCoordinatesUtils.SphericalToCartesian(_sphericalCoord);
        ConstrainToSphere();
    }

    public void SetVelocity(Vector3 velocity)
    {
        // Set the velocity while constraining it to the sphere, reset angular velocity to zero
        Rig.angularVelocity = Vector3.zero;
        Rig.velocity = Vector3.ProjectOnPlane(velocity, transform.position.normalized);
    }
}
