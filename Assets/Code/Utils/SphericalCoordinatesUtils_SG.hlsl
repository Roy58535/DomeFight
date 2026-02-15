
// Shader-graph friendly, self-contained spherical helpers
// Coordinate convention:
//   spherical.x = radius (r)
//   spherical.y = azimuth theta (theta)
//   spherical.z = polar   phi   (phi)
// Cartesian ordering: float3(x, y, z)

#ifndef SPHERICAL_UTILS_SG_INCLUDED
#define SPHERICAL_UTILS_SG_INCLUDED

static const float EPSILON_SPH = 1e-6;

// Core implementations (copied here so Shader Graph can load a single file)
inline float3 SphericalToCartesian_impl(float3 spherical)
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

inline float3 CartesianToSpherical_impl(float3 cartesian)
{
    float r = length(cartesian);
    if (r <= EPSILON_SPH)
    {
        // degenerate: at origin. return zeros.
        return float3(0.0, 0.0, 0.0);
    }

    // atan2(z, x) to match SphericalToCartesian_impl
    float theta = atan2(cartesian.z, cartesian.x);

    // clamp input to acos to avoid NaNs when numerical error slightly exceeds [-1,1]
    float cosPhi = clamp(cartesian.y / r, -1.0, 1.0);
    float phi = acos(cosPhi);

    return float3(r, theta, phi);
}

// Shader Graph wrappers: use these exact names in the Custom Function node
inline float3 SphericalToCartesian_float(float3 spherical)
{
    return SphericalToCartesian_impl(spherical);
}

inline float3 CartesianToSpherical_float(float3 cartesian)
{
    return CartesianToSpherical_impl(cartesian);
}

#endif // SPHERICAL_UTILS_SG_INCLUDED