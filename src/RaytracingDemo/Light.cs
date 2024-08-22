using System.Collections.Generic;

namespace RaytracingDemo;

public readonly struct LightSample(Vector attenuation, Vector direction)
{
    public readonly Vector Attenuation = attenuation;
    public readonly Vector Direction = direction;
}

public interface ILight
{
    /// <summary>
    /// Check if light can illuminate a point.
    /// true if there is clear path between point and light.
    /// false if there is an obstruction between point and light.
    /// </summary>
    bool CanIlluminate(Vector point, IEnumerable<IHittable> hittables, out LightSample sample);
}

public class PointLight : ILight
{
    public Vector Position;
    public Vector Color;
    public double Strength;

    public PointLight(Vector position, Vector color, double strength)
    {
        Position = position;
        Color = color;
        Strength = strength;
    }

    public bool CanIlluminate(Vector point, IEnumerable<IHittable> hittables, out LightSample sample)
    {
        var pointToLight = Position - point;
        var direction = pointToLight.Normalized;
        var ray = new Ray(point, direction);
        var limit = new Interval(Common.Epsilon, pointToLight.Magnitude);
        foreach (var hittable in hittables)
            if (hittable.Hit(in ray, in limit, out var _))
            {
                sample = default;
                return false;
            }
        var attenuation = Color * Strength / pointToLight.MagnitudeSqr;
        sample = new LightSample(attenuation , direction);
        return true;
    }
}

