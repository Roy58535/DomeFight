// HLSL implementations of the helpers in Assets/Code/Utils/SphericalCoordinatesUtils.cs
// Coordinate convention:
//   spherical.x = radius (r)
//   spherical.y = azimuth theta (theta)
//   spherical.z = polar   phi   (phi)
// Cartesian ordering: float3(x, y, z)
//
// Matches the C# implementation: 
//   x = r * cos(theta) * sin(phi)
//   z = r * sin(theta) * sin(phi)
//   y = r * cos(phi)

#ifndef SPHERICAL_UTILS_INCLUDED
#define SPHERICAL_UTILS_INCLUDED

static const float EPSILON_SPH = 1e-6;

inline float3 SphericalToCartesian_float(float3 spherical)
{
    float r = spherical.x;
    float theta = spherical.y;
    float phi = spherical.z;

    float sphi = sin(phi);
    float x = r * cos(theta) * sphi;
    float z = r * sin(theta) * sphi;
    float y = r * cos(phi);

    return float3(x, y, z);
}

inline float3 CartesianToSpherical_float(float3 cartesian)
{
    float r = length(cartesian);
    if (r <= EPSILON_SPH)
    {
        // degenerate: at origin. return zeros.
        return float3(0.0, 0.0, 0.0);
    }

    // HLSL atan2(y, x) -> here we want atan2(z, x) to match SphericalToCartesian
    float theta = atan2(cartesian.z, cartesian.x);

    // clamp input to acos to avoid NaNs when numerical error slightly exceeds [-1,1]
    float cosPhi = clamp(cartesian.y / r, -1.0, 1.0);
    float phi = acos(cosPhi);

    return float3(r, theta, phi);
}

#endif // SPHERICAL_UTILS_INCLUDED