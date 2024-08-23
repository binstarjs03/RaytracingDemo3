using System;
using System.Collections.Generic;

namespace RaytracingDemo;

public readonly struct RenderOption(Camera camera, Framebuffer framebuffer, in Interval culling, IEnumerable<IHittable> hittables, IEnumerable<ILight> lights, Random random, int maxSamples, int maxBounces)
{
    public readonly Camera Camera = camera;
    public readonly Framebuffer Framebuffer = framebuffer;
    public readonly Interval Culling = culling;
    public readonly IEnumerable<IHittable> Hittables = hittables;
    public readonly IEnumerable<ILight> Lights = lights;
    public readonly Random Random = random;
    public readonly int MaxSamples = (int)Math.Ceiling(Math.Sqrt(maxSamples));
    public readonly int MaxBounces = maxBounces;
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

        var maxSamples = option.MaxSamples * option.MaxSamples;

        for (var rasterY = 0; rasterY < height; rasterY++)
        {
            Console.WriteLine($"Scanlines remaining: {height - rasterY}");
            for (var rasterX = 0; rasterX < width; rasterX++)
            {
                var index = rasterX + rasterY * framebuffer.Height;
                ref var diffuseRaster = ref framebuffer.DiffuseBuffer[index];
                ref var normalRaster = ref framebuffer.NormalBuffer[index];
                ref var zRaster = ref framebuffer.ZBuffer[index];

                for (var i = 0; i < maxSamples; i++)
                {
                    var cameraRay = GenerateCameraRay(rasterX, rasterY, i, in option);
                    if (TryIntersectNearest(in cameraRay, in option.Culling, hittables, out var info))
                    {
                        var direct = SampleDirect(in info, hittables, lights);
                        var indirect = SampleIndirect(in option, in cameraRay, in info, info.Material.Albedo, depth: 0);
                        var attenuation = (direct + indirect) * info.Material.Albedo;
                        diffuseRaster += attenuation;
                        normalRaster += (info.Normal + 1) * 0.5;
                        zRaster += 1 - (-info.Hitpoint.Z - minCull) / (maxCull - minCull);
                    }
                    else
                    {
                        diffuseRaster += new Vector((double)rasterX / width, (double)rasterY / framebuffer.Height, 0);
                        normalRaster += Vector.Zero;
                        zRaster += Vector.Zero;
                    }
                }

                diffuseRaster /= maxSamples;
                normalRaster /= maxSamples;
                zRaster /= maxSamples;
            }
        }
        Console.WriteLine("Finished");
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

    private static Vector SampleIndirect(in RenderOption option, in Ray ray, in HitInfo info, in Vector attenuation, int depth)
    {
        // absorbed completely
        if (!info.Material.TryScatter(in ray, in info, out var scattered, option.Random))
            return Vector.Zero;
        // goes into infinity
        if (!TryIntersectNearest(in scattered, option.Culling, option.Hittables, out var scatterInfo))
            return Vector.Zero;
        var localAttenuation = attenuation * scatterInfo.Material.Albedo;
        var direct = SampleDirect(in scatterInfo, option.Hittables, option.Lights);
        var indirect = depth < option.MaxBounces
            ? SampleIndirect(in option, in scattered, in scatterInfo, localAttenuation, depth + 1)
            : Vector.Zero;
        return (direct + indirect) * localAttenuation;

    }

    private static Ray GenerateCameraRay(double rasterX, double rasterY, int i, in RenderOption option)
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

        // move random range to -0.5, 0.5

        var maxSamples = (double)option.MaxSamples;
        var halfDistance = 0.5 / maxSamples / 2;
        var xoff = i % maxSamples / maxSamples;
        var yoff = Math.Floor(i / maxSamples) / maxSamples;

        rasterX += xoff + halfDistance;
        rasterY += yoff + halfDistance;

        var xndc = rasterX / framebuffer.Width;
        var yndc = rasterY / framebuffer.Height;

        var xscreen = (xndc * 2 - 1) * screenLen * aspectRatio;
        var yscreen = (1 - yndc * 2) * screenLen;
        var pixelCenter = new Vector(xscreen, yscreen, -1);

        var origin = camera.Transformation.Transform(Vector.Zero);
        var direction = camera.Transformation.Transform(in pixelCenter).Normalized;
        return new Ray(origin, direction);
    }
}