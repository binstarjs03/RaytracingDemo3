using System;
using System.Collections.Generic;

namespace RaytracingDemo;

public readonly struct RenderOption(Interval cullInterval)
{
    public readonly Interval CullInterval = cullInterval;
}

public interface IRenderer
{
    void Render(ref readonly Camera camera, in RenderOption option, IEnumerable<IHittable> hittables, IEnumerable<ILight> lights);
}

public class Renderer : IRenderer
{
    public void Render(ref readonly Camera camera, in RenderOption option, IEnumerable<IHittable> hittables, IEnumerable<ILight> lights)
    {
        var minCull = option.CullInterval.Min;
        var maxCull = option.CullInterval.Max;
        for (var rasterY = 0; rasterY < camera.RasterHeight; rasterY++)
            for (var rasterX = 0; rasterX < camera.RasterWidth; rasterX++)
            {
                var index = rasterX + rasterY * camera.RasterWidth;
                ref var diffuseRaster = ref camera.Framebuffer.DiffuseBuffer[index];
                ref var normalRaster = ref camera.Framebuffer.NormalBuffer[index];
                ref var zRaster = ref camera.Framebuffer.ZBuffer[index];
                var cameraRay = GenerateCameraRay(rasterX, rasterY, in camera);
                if (TryIntersectNearest(in cameraRay, in option.CullInterval, hittables, out var info))
                {
                    diffuseRaster = SampleDirect(in info, hittables, lights) + SampleIndirect(in info, hittables, lights);
                    normalRaster = (info.Normal + 1) * 0.5;
                    zRaster = 1 - (-info.Hitpoint.Z - minCull) / (maxCull - minCull);
                }
                else
                {
                    diffuseRaster = new Vector((double)rasterX / camera.RasterWidth, (double)rasterY / camera.RasterHeight, 0);
                    normalRaster = Vector.Zero;
                    zRaster = Vector.Zero;
                }
            }
    }

    private static bool TryIntersectNearest(in Ray ray, in Interval cullLimit, IEnumerable<IHittable> hittables, out HitInfo info)
    {
        var localLimit = cullLimit;
        var wasHit = false;
        var localInfo = default(HitInfo);
        foreach (var hittable in hittables)
            if (hittable.Hit(in ray, in localLimit, out var tempInfo))
            {
                localLimit = new Interval(localLimit.Min, tempInfo.Distance);
                wasHit = true;
                localInfo = tempInfo;
            }
        info = localInfo;
        return wasHit;
    }

    private static Vector SampleDirect(in HitInfo info, IEnumerable<IHittable> hittables, IEnumerable<ILight> lights)
    {
        var finalAttenuation = Vector.Zero;
        foreach (var light in lights)
            if (light.CanIlluminate(info.Hitpoint, hittables, out var sample))
                finalAttenuation += sample.Attenuation * Vector.Dot(info.Normal, sample.Direction);
        return finalAttenuation;
    }

    private static Vector SampleIndirect(in HitInfo info, IEnumerable<IHittable> hittables, IEnumerable<ILight> lights)
    {
        return Vector.Zero;
    }

    private static Ray GenerateCameraRay(int rasterX, int rasterY, ref readonly Camera camera)
    {
        // conventionally:
        // - focus length is 1
        // - screen length is 2, 1 for each half(-axis and +axis)
        // at screen length 2, fov is 90d
        // for arbitrary screen length, just use tan(fov)
        var fovrad = camera.FieldOfView.ToRadian();
        var screenLen = Math.Tan(fovrad / 2) * 2;
        var aspectRatio = (double)camera.RasterWidth / camera.RasterHeight;

        var xndc = (rasterX + 0.5) / camera.RasterWidth;
        var yndc = (rasterY + 0.5) / camera.RasterHeight;

        var xscreen = (xndc * 2 - 1) * screenLen * aspectRatio;
        var yscreen = (1 - yndc * 2) * screenLen;
        var pixelCenter = new Vector(xscreen, yscreen, -1);

        var origin = camera.Transformation.Transform(Vector.Zero);
        var direction = camera.Transformation.Transform(in pixelCenter).Normalized;
        return new Ray(origin, direction);
    }
}