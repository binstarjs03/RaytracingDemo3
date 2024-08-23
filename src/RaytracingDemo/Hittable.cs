using System;

namespace RaytracingDemo;

public interface IHittable
{
    IMaterial Material { get; }
    bool Hit(in Ray incoming, in Interval limit, out HitInfo info);
}

public class Sphere(Vector center, double radius, IMaterial material, string name) : IHittable
{
    private readonly Vector _center = center;
    private readonly double _radius = radius;
    public IMaterial Material { get; set; } = material;

    public string Name { get; set; } = name;

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
        var isFront = Vector.Dot(in incoming.Direction, in normal) < 0;
        // add small bias to avoid hitpoint inside/below surface error
        info = new HitInfo(hitpoint + normal * Common.Epsilon, normal, distance: intersection, isFront, this);
        return true;
    NotHit:
        info = default;
        return false;
    }
}

public class TriMesh : IHittable
{
    private readonly Vector[] _positions;
    private readonly int[] _indices;
    private readonly BoundingBox _boundingBox;

    public TriMesh(Vector[] positions, int[] indices, IMaterial material, string name)
    {
        if (positions.Length < 3)
            throw new ArgumentException("Vertex array must be larger than 3", nameof(positions));
        if (indices.Length % 3 != 0)
            throw new ArgumentException("Index array must be divisible by 3", nameof(positions));
        _positions = positions;
        _indices = indices;
        Material = material;
        _boundingBox = CalculateBoundingBox(positions);
        Name = name;
    }
    
    public IMaterial Material { get; set; }

    private static BoundingBox CalculateBoundingBox(Vector[] positions)
    {
        var minX = double.PositiveInfinity;
        var minY = double.PositiveInfinity;
        var minZ = double.PositiveInfinity;
        var maxX = double.NegativeInfinity;
        var maxY = double.NegativeInfinity;
        var maxZ = double.NegativeInfinity;
        for (var i = 0; i < positions.Length; i++)
        {
            ref var point = ref positions[i];
            if (point.X < minX)
                minX = point.X;
            if (point.Y < minY)
                minY = point.Y;
            if (point.Z < minZ)
                minZ = point.Z;
            if (point.X > maxX)
                maxX = point.X;
            if (point.Y > maxY)
                maxY = point.Y;
            if (point.Z > maxZ)
                maxZ = point.Z;
        }
        return new BoundingBox(minX, minY, minZ, maxX, maxY, maxZ);
    }

    public string Name { get; set; }

    public bool Hit(in Ray incoming, in Interval limit, out HitInfo info)
    {
        if (!_boundingBox.Intersect(in incoming))
        {
            info = default;
            return false;
        }
        var localLimit = limit;
        var wasHit = false;
        var localInfo = default(HitInfo);
        var tricount = _indices.Length / 3;
        for (var i = 0; i < tricount; i++)
        {
            var indexStart = i * 3;
            ref var p0 = ref _positions[_indices[indexStart + 0]];
            ref var p1 = ref _positions[_indices[indexStart + 1]];
            ref var p2 = ref _positions[_indices[indexStart + 2]];
            if (Triangle.Hit(in incoming, in localLimit, in p0, in p1, in p2, out var tempInfo, this))
            {
                localLimit = new Interval(localLimit.Min, localInfo.Distance);
                wasHit = true;
                localInfo = tempInfo;
            }
        }
        info = localInfo;
        return wasHit;
    }
}

public static class Triangle
{
    public static Vector Normal(in Vector p0, in Vector p1, in Vector p2)
    {
        return Vector.Cross(p1 - p0, p2 - p0).Normalized;
    }

    public static bool Hit(in Ray incoming, in Interval limit, in Vector p0, in Vector p1, in Vector p2, out HitInfo info, IHittable hittable)
    {
        var normal = Normal(in p0, in p1, in p2);

        // check if incoming ray is parallel to the plane or perpendicular to normal
        var nDotIn = Vector.Dot(in normal, in incoming.Direction);
        var isParallel = Math.Abs(nDotIn) < double.Epsilon;
        if (isParallel)
            goto NoHit;

        // find hitpoint between incoming ray and plane
        var d = -Vector.Dot(in normal, in p0);
        var intersection = -(Vector.Dot(in normal, in incoming.Origin) + d) / nDotIn;
        if (intersection < 0)
            goto NoHit; // triangle is behind
        if (!limit.Inside(intersection))
            goto NoHit;
        var hitpoint = incoming.At(intersection);

        // check if hitpoint is inside triangle

        // test for edge v01
        var vedge = p1 - p0; // vector between two vertices
        var vp = hitpoint - p0; // vector between hitpoint and vertex
        var vT = Vector.Cross(in vedge, in vp); // vector perpendicular to triangle's plane
        if (Vector.Dot(in normal, in vT) < 0)
            goto NoHit;

        // test for edge v12
        vedge = p2 - p1;
        vp = hitpoint - p1;
        vT = Vector.Cross(in vedge, in vp);
        if (Vector.Dot(in normal, in vT) < 0)
            goto NoHit;

        // test for edge v20
        vedge = p0 - p2;
        vp = hitpoint - p2;
        vT = Vector.Cross(in vedge, in vp);
        if (Vector.Dot(in normal, in vT) < 0)
            goto NoHit;

        var isFront = Vector.Dot(in incoming.Direction, in normal) < 0;
        info = new HitInfo(hitpoint, normal, intersection, isFront, hittable);
        return true;

    NoHit:
        info = default;
        return false;
    }
}
