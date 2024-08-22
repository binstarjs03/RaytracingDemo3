using System;

namespace RaytracingDemo;

public struct Transformation(in Vector position, in Vector rotation, in Vector scale)
{
    public Vector Position = position;
    public Vector Rotation = rotation;
    public Vector Scale = scale;

    public static Transformation Default => new(Vector.Zero, Vector.Zero, Vector.Unit);

    public readonly Vector Transform(in Vector point)
    {
        // scale
        var x = point.X * Scale.X;
        var y = point.Y * Scale.Y;
        var z = point.Z * Scale.Z;

        var xsin = Math.Sin(Rotation.X);
        var ysin = Math.Sin(Rotation.Y);
        var zsin = Math.Sin(Rotation.Z);

        var xcos = Math.Cos(Rotation.X);
        var ycos = Math.Cos(Rotation.Y);
        var zcos = Math.Cos(Rotation.Z);

        // rotate along x axis
        var xr = x;
        var yr = y * xcos - z * xsin;
        var zr = y * xsin + z * xcos;
        x = xr;
        y = yr;
        z = zr;

        // rotate along y axis
        xr = x * ycos + z * ysin;
        yr = y;
        zr = -x * ysin + z * ycos;
        x = xr;
        y = yr;
        z = zr;

        // rotate along z axis
        xr = x * zcos - y * zsin;
        yr = x * zsin + y * zcos;
        zr = z;
        x = xr;
        y = yr;
        z = zr;

        // translate
        x += Position.X;
        y += Position.Y;
        z += Position.Z;

        return new Vector(x, y, z);
    }

    public readonly Vector TransformInverse(in Vector point)
    {
        // translate
        var x = point.X - Position.X;
        var y = point.Y - Position.Y;
        var z = point.Z - Position.Z;

        var xsin = Math.Sin(-Rotation.X);
        var ysin = Math.Sin(-Rotation.Y);
        var zsin = Math.Sin(-Rotation.Z);

        var xcos = Math.Cos(-Rotation.X);
        var ycos = Math.Cos(-Rotation.Y);
        var zcos = Math.Cos(-Rotation.Z);

        // rotate along z axis
        var xr = x * zcos - y * zsin;
        var yr = x * zsin + y * zcos;
        var zr = z;
        x = xr;
        y = yr;
        z = zr;

        // rotate along y axis
        xr = x * ycos + z * ysin;
        yr = y;
        zr = -x * ysin + z * ycos;
        x = xr;
        y = yr;
        z = zr;

        // rotate along x axis
        xr = x;
        yr = y * xcos - z * xsin;
        zr = y * xsin + z * xcos;
        x = xr;
        y = yr;
        z = zr;

        // scale
        x /= Scale.X;
        y /= Scale.Y;
        z /= Scale.Z;

        return new Vector(x, y, z);
    }
}
