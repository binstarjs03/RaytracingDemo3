using System;
using System.Collections.Generic;
using System.IO;
namespace RaytracingDemo;

class Program
{

    static void Main(string[] args)
    {
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
            new Sphere(new Vector(0.0, 0.0, -1.0), radius: 0.5),
            new Sphere(new Vector(0.5, 0.0, -1.0), radius: 0.25),
            new TriMesh(positions, indices),
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
            // new PointLight(new Vector(-1, 1, 1), Vector.Unit, 0.5),
        };

        // setup render config
        var multiplier = 40;
        var framebuffer = new Framebuffer(width: 10 * multiplier, height: 10 * multiplier);
        var renderer = new Renderer();
        var camera = new Camera(fieldOfView: 50, Transformation.Default);
        var culling = new Interval(min: 0.1, max: 20);
        var random = new Random(0);
        var option = new RenderOption(camera, framebuffer, in culling, hittables, lights, random, 16);
        
        // hit F12
        renderer.Render(in option);

        // setup outputs
        using var diffuseWriter = new StreamWriter("outDiffuse.ppm");
        using var normalWriter = new StreamWriter("outNormal.ppm");
        using var zWriter = new StreamWriter("outZ.ppm");
        framebuffer.ExportToPPM(diffuseWriter, normalWriter, zWriter);
    }
}