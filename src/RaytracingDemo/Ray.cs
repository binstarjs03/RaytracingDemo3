namespace RaytracingDemo;

public readonly struct Ray(Vector origin, Vector direction)
{
    public readonly Vector Origin = origin;
    public readonly Vector Direction = direction;

    public readonly Vector At(double distance)
    {
        return Origin + (Direction * distance);
    }
}