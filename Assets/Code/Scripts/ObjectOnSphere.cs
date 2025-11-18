using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOnSphere : MonoBehaviour
{
    public static float GravityStrength = 20f;
    public static float Radius = 10f;

    [SerializeField] private bool _useGravity = false;
    [SerializeField] private Vector3 _sphericalCoord;
    protected Vector3 SphericalCoord => _sphericalCoord;
    protected Vector3 AzimuthDir => _azimuthDir;
    protected Vector3 RadialDir => _radialDir;
    protected Vector3 SurfaceDownDir => _surfaceDownDir;

    protected Rigidbody Rig;

    private Vector3 _azimuthDir;
    private Vector3 _radialDir;
    private Vector3 _surfaceDownDir;

    protected virtual void Start()
    {
        // Initialize on sphere surface
        ConstrainToSphere();
        _sphericalCoord = SphericalCoordinatesUtils.CartesianToSpherical(transform.position);
        transform.position = SphericalCoordinatesUtils.SphericalToCartesian(_sphericalCoord);
        Rig = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        // Constrain position and orientation
        transform.forward = transform.position;
        ConstrainToSphere();
        _sphericalCoord = SphericalCoordinatesUtils.CartesianToSpherical(transform.position);
    }

    protected virtual void FixedUpdate()
    {
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
        Vector3 dir = transform.position.normalized;
        transform.position = dir * Radius;
    }
}
