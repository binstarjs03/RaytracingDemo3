using System;
using System.Collections.Generic;

namespace RaytracingDemo;

public readonly struct RenderOption(Camera camera, Framebuffer framebuffer, in Interval culling, IEnumerable<IHittable> hittables, IEnumerable<ILight> lights, int maxSamples)
{
    public readonly Camera Camera = camera;
    public readonly Framebuffer Framebuffer = framebuffer;
    public readonly Interval Culling = culling;
    public readonly IEnumerable<IHittable> Hittables = hittables;
    public readonly IEnumerable<ILight> Lights = lights;
    public readonly int MaxSamples = maxSamples;
}

public interface IRenderer
{
    void Render(in RenderOption option);
}

public class Renderer : IRenderer
{
    public void Render(in RenderOption option)
    {
        ref readonly var framebuffer = ref option.Framebuffer;
        var hittables = option.Hittables;
        var lights = option.Lights;
        
        var width = framebuffer.Width;
        var height = framebuffer.Height;
        
        var minCull = option.Culling.Min;
        var maxCull = option.Culling.Max;
        
        for (var rasterY = 0; rasterY < height; rasterY++)
            for (var rasterX = 0; rasterX < width; rasterX++)
            {
                var index = rasterX + rasterY * framebuffer.Height;
                ref var diffuseRaster = ref framebuffer.DiffuseBuffer[index];
                ref var normalRaster = ref framebuffer.NormalBuffer[index];
                ref var zRaster = ref framebuffer.ZBuffer[index];
                var cameraRay = GenerateCameraRay(rasterX, rasterY, in option);
                if (TryIntersectNearest(in cameraRay, in option.Culling, hittables, out var info))
                {
                    diffuseRaster = SampleDirect(in info, hittables, lights) + SampleIndirect(in info, hittables, lights);
                    normalRaster = (info.Normal + 1) * 0.5;
                    zRaster = 1 - (-info.Hitpoint.Z - minCull) / (maxCull - minCull);
                }
                else
                {
                    diffuseRaster = new Vector((double)rasterX / width, (double)rasterY / framebuffer.Height, 0);
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

    private static Ray GenerateCameraRay(int rasterX, int rasterY, in RenderOption option)
    {
        // conventionally:
        // - focus length is 1
        // - screen length is 2, 1 for each half(-axis and +axis)
        // at screen length 2, fov is 90d
        // for arbitrary screen length, just use tan(fov)
        ref readonly var camera = ref option.Camera;
        ref readonly var framebuffer = ref option.Framebuffer;
        var fovrad = camera.FieldOfView.ToRadian();
        var screenLen = Math.Tan(fovrad / 2) * 2;
        var aspectRatio = (double)framebuffer.Width / framebuffer.Height;

        var xndc = (rasterX + 0.5) / framebuffer.Width;
        var yndc = (rasterY + 0.5) / framebuffer.Height;

        var xscreen = (xndc * 2 - 1) * screenLen * aspectRatio;
        var yscreen = (1 - yndc * 2) * screenLen;
        var pixelCenter = new Vector(xscreen, yscreen, -1);

        var origin = camera.Transformation.Transform(Vector.Zero);
        var direction = camera.Transformation.Transform(in pixelCenter).Normalized;
        return new Ray(origin, direction);
    }
}