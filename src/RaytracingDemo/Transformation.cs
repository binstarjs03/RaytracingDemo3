namespace RaytracingDemo;

public struct Transformation(in Vector position, in Vector rotation, in Vector scale)
{
    public Vector Position = position;
    public Vector Rotation = rotation;
    public Vector Scale = scale;

    public static Transformation Default => new(Vector.Zero, Vector.Zero, Vector.Unit);
}
