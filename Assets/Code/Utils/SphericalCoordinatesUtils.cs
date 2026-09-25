using UnityEngine;

public static class SphericalCoordinatesUtils
{
    public static Vector3 SphericalToCartesian(Vector3 sphericalInput)
    {
        // Spherical coordinates are represented as (radius, azimuth, polar)
        if (sphericalInput != null)
        {
            float x = sphericalInput.x * Mathf.Cos(sphericalInput.y) * Mathf.Sin(sphericalInput.z);
            float z = sphericalInput.x * Mathf.Sin(sphericalInput.y) * Mathf.Sin(sphericalInput.z);
            float y = sphericalInput.x * Mathf.Cos(sphericalInput.z);
            Vector3 cartesian = new Vector3(x, y, z);
            return cartesian;
        }
        return Vector3.zero;
    }

    public static Vector3 CartesianToSpherical(Vector3 cartesianInput)
    {
        // Spherical coordinates are represented as (radius, azimuth, polar)
        if (cartesianInput != null)
        {
            float radius = Mathf.Sqrt(cartesianInput.x * cartesianInput.x + cartesianInput.y * cartesianInput.y + cartesianInput.z * cartesianInput.z);
            float theta = Mathf.Atan2(cartesianInput.z, cartesianInput.x);
            float phi = Mathf.Acos(cartesianInput.y / radius);
            Vector3 spherical = new Vector3(radius, theta, phi);
            return spherical;
        }
        return Vector3.zero;
    }
}
