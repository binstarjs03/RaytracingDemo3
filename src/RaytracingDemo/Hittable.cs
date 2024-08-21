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
public readonly struct Triangle : IHittable
{
    public readonly Vector V0;
    public readonly Vector V1;
    public readonly Vector V2;

    public Vector Normal => Vector.Cross(V1 - V0, V2 - V0);

    public bool Hit(in Ray incoming, in Interval limit, out HitInfo info)
    {
        var normal = Normal;

        // check if incoming ray is parallel to the plane or perpendicular to normal
        var nDotIn = Vector.Dot(in normal, in incoming.Direction);
        var isParallel = nDotIn < double.Epsilon;
        if (isParallel)
            goto NoHit;

        // find hitpoint between incoming ray and plane
        var d = -Vector.Dot(in normal, in V0);
        var intersect = -(Vector.Dot(in normal, in incoming.Origin) + d) / nDotIn;
        if (intersect < 0)
            goto NoHit; // triangle is behind
        var hitpoint = incoming.At(intersect);

        // check if hitpoint is inside triangle
        var vedge = Vector.Zero; // vector between two vertices
        var vp = Vector.Zero; // vector between hitpoint and vertex
        var vT = Vector.Zero; // vector perpendicular to triangle's plane

        // test for edge v01
        vedge = V1 - V0;
        vp = hitpoint - V0;
        vT = Vector.Cross(in vedge, in vp);
        if (Vector.Dot(in normal, in vT) < 0)
            goto NoHit;

        // test for edge v12
        vedge = V2 - V1;
        vp = hitpoint - V1;
        vT = Vector.Cross(in vedge, in vp);
        if (Vector.Dot(in normal, in vT) < 0)
            goto NoHit;

        // test for edge v20
        vedge = V0 - V2;
        vp = hitpoint - V2;
        vT = Vector.Cross(in vedge, in vp);
        if (Vector.Dot(in normal, in vT) < 0)
            goto NoHit;

        var isFront = Vector.Dot(in incoming.Direction, in normal) < 0;
        info = new HitInfo(hitpoint, normal, intersect, isFront);
        return true;

    NoHit:
        info = new HitInfo();
        return false;
    }
}
