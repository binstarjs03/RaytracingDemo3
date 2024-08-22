namespace RaytracingDemo;

public readonly struct HitInfo(Vector hitpoint, Vector normal, double distance, bool isFront, IMaterial material)
{
    public readonly Vector Hitpoint = hitpoint;
    public readonly Vector Normal = normal;
    public readonly double Distance = distance;
    public readonly bool IsFront = isFront;
    public readonly IMaterial Material = material;
}