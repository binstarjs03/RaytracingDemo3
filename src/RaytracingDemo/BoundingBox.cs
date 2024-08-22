namespace RaytracingDemo;

public readonly struct BoundingBox
{
    public readonly Vector Min;
    public readonly Vector Max;

    public BoundingBox(Vector min, Vector max)
    {
        Min = min;
        Max = max;
    }

    public BoundingBox(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
    {
        Min = new Vector(minX, minY, minZ);
        Max = new Vector(maxX, maxY, maxZ);
    }

    public bool Intersect(in Ray ray)
    {
        var tmin = (Min.X - ray.Origin.X) / ray.Direction.X;
        var tmax = (Max.X - ray.Origin.X) / ray.Direction.X;
        if (tmin > tmax)
            (tmin, tmax) = (tmax, tmin);

        var tymin = (Min.Y - ray.Origin.Y) / ray.Direction.Y;
        var tymax = (Max.Y - ray.Origin.Y) / ray.Direction.Y;
        if (tymin > tymax)
            (tymin, tymax) = (tymax, tymin);

        if ((tmin > tymax) || (tymin > tmax))
            return false;
        if (tymin > tmin)
            tmin = tymin;
        if (tymax < tmax)
            tmax = tymax;

        var tzmin = (Min.Z - ray.Origin.Z) / ray.Direction.Z;
        var tzmax = (Max.Z - ray.Origin.Z) / ray.Direction.Z;
        if (tzmin > tzmax)
            (tzmin, tzmax) = (tzmax, tzmin);

        if ((tmin > tzmax) || (tzmin > tmax))
            return false;
        // if (tzmin > tmin)
        //     tmin = tzmin;
        if (tzmax < tmax)
            tmax = tzmax;

        return tmax >= 0;
    }

    // public bool InsideOrEq(in Vector point)
    // {
    // return X.InsideOrEq(point.X)
    // && Y.InsideOrEq(point.Y)
    // && Z.InsideOrEq(point.Z);
    // }

    // public bool Intersect(in Ray ray)
    // {
    //     //if (InsideOrEq(in ray.Origin))
    //     //    return true;
    //     return IntersectPlane(in ray, 'x')
    //         || IntersectPlane(in ray, 'y')
    //         || IntersectPlane(in ray, 'z');
    // }


    // private bool IntersectPlane(in Ray ray, char axis)
    // {
    //     var dummy = 0d;
    //     ref readonly var rayDirection = ref dummy;
    //     ref readonly var rayOrigin = ref dummy;
    //     ref readonly var minSubjectAxis = ref dummy;
    //     ref readonly var minAxis0 = ref dummy;
    //     ref readonly var maxAxis0 = ref dummy;
    //     ref readonly var minAxis1 = ref dummy;
    //     ref readonly var maxAxis1 = ref dummy;

    //     if (axis == 'x')
    //     {
    //         rayDirection = ref ray.Direction.X;
    //         rayOrigin = ref ray.Origin.X;
    //         minSubjectAxis = ref X.Min;
    //         minAxis0 = ref Y.Min;
    //         maxAxis0 = ref Y.Max;
    //         minAxis1 = ref Z.Min;
    //         maxAxis1 = ref Z.Max;
    //     }
    //     else if (axis == 'y')
    //     {
    //         rayDirection = ref ray.Direction.Y;
    //         rayOrigin = ref ray.Origin.Y;
    //         minSubjectAxis = ref Y.Min;
    //         minAxis0 = ref X.Min;
    //         maxAxis0 = ref X.Max;
    //         minAxis1 = ref Z.Min;
    //         maxAxis1 = ref Z.Max;
    //     }
    //     else if (axis == 'z')
    //     {
    //         rayDirection = ref ray.Direction.Z;
    //         rayOrigin = ref ray.Origin.Z;
    //         minSubjectAxis = ref Z.Min;
    //         minAxis0 = ref X.Min;
    //         maxAxis0 = ref X.Max;
    //         minAxis1 = ref Y.Min;
    //         maxAxis1 = ref Y.Max;
    //     }
    //     else
    //         throw new Exception();

    //     // check if ray is parallel with plane
    //     if (Math.Abs(ray.Direction.Z) < Common.Epsilon)
    //         return false;

    //     // check if intersection behind the ray
    //     var t = (Z.Min - ray.Origin.Z) / ray.Direction.Z;
    //     if (t < Common.Epsilon)
    //         return false;

    //     // check if intersection point is within bounds
    //     var p = ray.At(t);
    //     if (p.X >= X.Min && p.X <= X.Max &&
    //         p.Y >= Y.Min && p.Y <= Y.Max)
    //         return true;
    //     return false;
    // }
}