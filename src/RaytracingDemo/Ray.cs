using System.Diagnostics;

namespace RaytracingDemo;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct Ray(Vector origin, Vector direction)
{
    public readonly Vector Origin = origin;
    public readonly Vector Direction = direction;

    public readonly Vector At(double distance)
    {
        return Origin + (Direction * distance);
    }

    public override string ToString()
    {
        return $"Origin: {Origin}; Direction: {Direction}";
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}