using System;

namespace RaytracingDemo;

public interface IHittable
{
    bool Hit(in Ray incoming, in Interval limit, out HitInfo info);
}

public class Sphere : IHittable
{
    private Vector _center;
    private double _radius;

    public bool Hit(in Ray incoming, in Interval limit, out HitInfo info)
    {
        var oc = _center - incoming.Origin;
        var a = Vector.Dot(in incoming.Direction, in incoming.Direction);
        var b = -2 * Vector.Dot(in incoming.Direction, in oc);
        var c = Vector.Dot(in oc, in oc) - (_radius * _radius);
        var det = b * b - 4 * a * c;
        if (det < 0)
            goto NotHit;
        var detSqrt = Math.Sqrt(det);
        var asqr = a * a;
        var intersection = (b - detSqrt) / asqr;
        if (!limit.Inside(intersection))
        {
            intersection = (b + detSqrt) / asqr;
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
