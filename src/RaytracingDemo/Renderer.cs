using System;
using System.Collections.Generic;
using System.IO;

namespace RaytracingDemo;

public interface IRenderer
{
    void Render(ref readonly Camera camera, IEnumerable<IHittable> hittables);
}

public class Renderer : IRenderer
{
    public void Render(ref readonly Camera camera, IEnumerable<IHittable> hittables)
    {
        for (var rasterY = 0; rasterY < camera.RasterHeight; rasterY++)
            for (var rasterX = 0; rasterX < camera.RasterWidth; rasterX++)
            {
                var index = rasterX + rasterY * camera.RasterWidth;
                ref var diffuseRaster = ref camera.Framebuffer.DiffuseBuffer[index];
                ref var normalRaster = ref camera.Framebuffer.NormalBuffer[index];
                var cameraRay = GenerateCameraRay(rasterX, rasterY, in camera);
                if (TryIntersectNearest(in cameraRay, hittables, out var info))
                {
                    diffuseRaster = Vector.Unit;
                    normalRaster = (info.Normal + 1) * 0.5;
                }
                else
                {
                    diffuseRaster = new Vector((double)rasterX / camera.RasterWidth, (double)rasterY / camera.RasterHeight, 0);
                    normalRaster = Vector.Zero;
                }
            }
    }

    private static bool TryIntersectNearest(in Ray ray, IEnumerable<IHittable> hittables, out HitInfo info)
    {
        var limit = new Interval(0.001, double.PositiveInfinity);
        var wasHit = false;
        var localInfo = new HitInfo();
        foreach (var hittable in hittables)
            if (hittable.Hit(in ray, in limit, out var tempInfo))
            {
                limit = new Interval(limit.Min, tempInfo.Distance);
                wasHit = true;
                localInfo = tempInfo;
            }
        info = localInfo;
        return wasHit;
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