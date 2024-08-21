using System;
using System.Collections.Generic;
using System.IO;

namespace RaytracingDemo;

public readonly struct RenderStream(StreamWriter composite, StreamWriter normal)
{
    public readonly StreamWriter Composite = composite;
    public readonly StreamWriter Normal = normal;

    public readonly void WriteHeader(int rasterWidth, int rasterHeight)
    {
        Composite.WriteLine("P3");
        Composite.WriteLine($"{rasterWidth} {rasterHeight}");
        Composite.WriteLine("255");
        Normal.WriteLine("P3");
        Normal.WriteLine($"{rasterWidth} {rasterHeight}");
        Normal.WriteLine("255");
    }
}

public interface IRenderer
{
    void Render(in RenderStream stream, ref readonly Camera camera, IEnumerable<IHittable> hittables);
}

public class Renderer : IRenderer
{
    public void Render(in RenderStream stream, ref readonly Camera camera, IEnumerable<IHittable> hittables)
    {
        stream.WriteHeader(camera.RasterWidth, camera.RasterHeight);
        for (var rasterY = 0; rasterY < camera.RasterHeight; rasterY++)
            for (var rasterX = 0; rasterX < camera.RasterWidth; rasterX++)
            {
                var cameraRay = GenerateCameraRay(rasterX, rasterY, in camera);
                if (TryIntersectNearest(in cameraRay, hittables, out var info))
                {
                    stream.Composite.WriteColor(Vector.Unit);
                    stream.Normal.WriteColor((info.Normal + 1) * 0.5);
                }
                else
                {
                    stream.Composite.WriteColor(new Vector((double)rasterX / camera.RasterWidth, (double)rasterY / camera.RasterHeight, 0));
                    stream.Normal.WriteColor(Vector.Zero);
                }
            }
    }

    private bool TryIntersectNearest(in Ray ray, IEnumerable<IHittable> hittables, out HitInfo info)
    {
        var limit = new Interval(0.001, double.PositiveInfinity);
        var wasHit = false;
        var tempInfo = new HitInfo();
        foreach (var hittable in hittables)
            if (hittable.Hit(in ray, in limit, out tempInfo))
            {
                limit = new Interval(limit.Min, tempInfo.Distance);
                wasHit = true;
            }
        info = tempInfo;
        return wasHit;
    }

    private Ray GenerateCameraRay(int rasterX, int rasterY, ref readonly Camera camera)
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