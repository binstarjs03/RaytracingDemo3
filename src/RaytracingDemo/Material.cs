using System;

namespace RaytracingDemo;

public interface IMaterial
{
    Vector Albedo{ get; }
    bool TryScatter(in Ray incoming, in HitInfo hit, out Ray scattered, Random random);
}

public class Lambertian(Vector albedo) : IMaterial
{
    public Vector Albedo { get; set; } = albedo;
    public bool TryScatter(in Ray incoming, in HitInfo hit, out Ray scattered, Random random)
    {
        var scatterDir = (hit.Normal + random.NextVectorOnHemisphere(in hit.Normal)).Normalized;
        scattered = new Ray(hit.Hitpoint, scatterDir);
        return true;
    }
}
