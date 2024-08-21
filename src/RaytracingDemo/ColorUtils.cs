using System;
using System.IO;

namespace RaytracingDemo;

public static class ColorUtils
{
    public static Vector ToGamma(this in Vector color)
    {
        var r = color.X > 0 ? Math.Sqrt(color.X) : 0;
        var g = color.Y > 0 ? Math.Sqrt(color.Y) : 0;
        var b = color.Z > 0 ? Math.Sqrt(color.Z) : 0;
        return new Vector(r, g, b);
    }

    public static void WriteColor(this StreamWriter writer, in Vector color)
    {
        var limit = new Interval(0, 0.999);
        var r = (int)(limit.Clamp(color.X) * 255);
        var g = (int)(limit.Clamp(color.Y) * 255);
        var b = (int)(limit.Clamp(color.Z) * 255);
        writer.WriteLine($"{r} {g} {b}");
    }
}