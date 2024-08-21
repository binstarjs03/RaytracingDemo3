using System;

namespace RaytracingDemo;

public interface IHittable
{
    bool Hit(in Ray incoming, in Interval limit, out HitInfo info);
}

public class Sphere(Vector center, double radius) : IHittable
{
    private Vector _center = center;
    private double _radius = radius;

    public bool Hit(in Ray incoming, in Interval limit, out HitInfo info)
    {
        var oc = _center - incoming.Origin;
        var a = incoming.Direction.MagnitudeSqr;
        var b = Vector.Dot(in incoming.Direction, in oc);
        var c = oc.MagnitudeSqr - _radius * _radius;
        var det = b * b - a * c;
        if (det < 0)
            goto NotHit;
        var detSqrt = Math.Sqrt(det);
        var intersection = (b - detSqrt) / a;
        if (!limit.Inside(intersection))
        {
            intersection = (b + detSqrt) / a;
            if (!limit.Inside(intersection))
                goto NotHit;
        }
        var hitpoint = incoming.At(intersection);
        var normal = (hitpoint - _center) / _radius;
        var isFront = Vector.Dot(incoming.Direction, normal) < 0;
        info = new HitInfo(hitpoint, normal, distance: intersection, isFront);
        return true;
    NotHit:
        info = new HitInfo();
        return false;

    }
}
