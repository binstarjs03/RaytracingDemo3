namespace RaytracingDemo;

public readonly struct HitInfo(Vector hitpoint, Vector normal, double distance, bool isFront, IHittable hittable)
{
    public readonly Vector Hitpoint = hitpoint;
    public readonly Vector Normal = normal;
    public readonly double Distance = distance;
    public readonly bool IsFront = isFront;
    public readonly IHittable Hittable = hittable;
}