using System;
using System.Collections.Generic;
using System.IO;
namespace RaytracingDemo;

class Program
{

    static void Main(string[] args)
    {
        var grayMat = new Lambertian(new Vector(0.8, 0.8, 0.8));
        var bluishMat = new Lambertian(new Vector(0.15, 0.4, 0.8));
        var reddishMat = new Lambertian(new Vector(0.8, 0.15, 0.15));

        // populate the scene
        Vector[] positions =
        {
            new(-1, -1, -2),
            new( 1, -1, -2),
            new( 0,  1, -2),

            new(-10, -0.5, -10),
            new( 10, -0.5, -10),
            new( 10, -0.5,  10),
            new(-10, -0.5,  10),

        };
        int[] indices =
        {
            // 0, 1, 2,
            5, 4, 3,
            3, 6, 5,
        };
        var hittables = new List<IHittable>()
        {
            new Sphere(new Vector(0.0, 0.0, -1.0), radius: 0.5, bluishMat, "Blue"),
            new Sphere(new Vector(0.5, 0.0, -1.0), radius: 0.25, reddishMat, "Red"),
            new TriMesh(positions, indices, grayMat, "Floor"),
        };

        // for (int i = 0; i < 100; i++)
        // {
        //     var x = Random.Shared.NextDouble() * 10 - 5;
        //     var y = Random.Shared.NextDouble() * 2;
        //     var z = Random.Shared.NextDouble() * 10 - 5;
        //     var r = Random.Shared.NextDouble() * 0.3 + 0.3;
        //     hittables.Add(new Sphere(new Vector(x, y, z), r));
        // }

        var lights = new List<ILight>
        {
            new PointLight(new Vector(0.6, 1.0, 0.0), Vector.Unit, 1.5),
            // new PointLight(new Vector(1, 1, -1), Vector.Unit, 1),
        };

        // setup render config
        var multiplier = 40;
        var framebuffer = new Framebuffer(width: 10 * multiplier, height: 10 * multiplier);
        var renderer = new Renderer();
        var camera = new Camera(fieldOfView: 50, 0, 0, Transformation.Default);
        var culling = new Interval(min: 0.1, max: 20);
        var random = new Random(0);
        var samples = 128;
        var bounces = 4;
        var option = new RenderOption(camera, framebuffer, in culling, hittables, lights, random, samples, bounces);

        // hit F12
        renderer.Render(in option);
        framebuffer.ExportToPPM($"-s{samples}b{bounces}");
    }
}