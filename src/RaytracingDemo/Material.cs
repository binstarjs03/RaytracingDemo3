using System;

namespace RaytracingDemo;

public interface IMaterial
{
    bool TryScatter(in Ray incoming, in HitInfo hit, out Ray scattered, Random random);
}

public class Lambertian(Vector albedo) : IMaterial
{
    public Vector Albedo = albedo;
    
    public bool TryScatter(in Ray incoming, in HitInfo hit, out Ray scattered, Random random)
    {
        var scatterDir = hit.Normal + random.NextVectorOnHemisphere(in hit.Normal);
        scattered = new Ray(hit.Hitpoint, scatterDir);
        return true;
    }
}
