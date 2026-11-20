using UnityEngine;

public static class BulletTrajectory
{
    // Use the same subdivision for projectile movement and trajectory previews.
    public static int SegmentCount(Vector3 velocity, float acceleration, float duration, float radius, float maxSegmentLength)
    {
        float maxDistance = Mathf.Max(0.01f, Mathf.Min(maxSegmentLength, radius * 2f * Mathf.Deg2Rad));
        return Mathf.Max(1, Mathf.CeilToInt((velocity.magnitude + acceleration * duration) * duration / maxDistance));
    }

    public static Vector3 Advance(Vector3 position, ref Vector3 velocity, float radius, float acceleration, float step)
    {
        // Apply tangent gravity before rotating position and velocity around the sphere.
        Vector3 normal = position / radius;
        Vector3 down = Vector3.ProjectOnPlane(Vector3.down, normal);
        if (down.sqrMagnitude > 0.000001f)
            velocity += down.normalized * acceleration * step;

        float speed = velocity.magnitude;
        if (speed <= 0.000001f)
            return position;

        Vector3 axis = Vector3.Cross(normal, velocity / speed).normalized;
        Quaternion turn = Quaternion.AngleAxis(speed * step / radius * Mathf.Rad2Deg, axis);
        velocity = turn * velocity;
        return (turn * normal).normalized * radius;
    }
}
