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
            new(-1, -1, -1),
            new( 1, -1, -1),
            new( 0,  1, -1),
        };
        int[] indices =
        {
            0, 1, 2,
        };
        var hittables = new List<IHittable>()
        {
            new Sphere(new Vector(0.0, 0.0, -1.0), radius: 0.5),
            new Sphere(new Vector(0.5, 0.0, -1.0), radius: 0.5),
            new TriMesh(positions, indices),
        };

        // setup render config
        var multiplier = 40;
        var framebuffer = new Framebuffer(width: 16 * multiplier, height: 9 * multiplier);
        var renderer = new Renderer();
        var camera = new Camera(framebuffer, fieldOfView: 90, Transformation.Default);
        var culling = new Interval(min: 0.1, max: 5);
        var option = new RenderOption(culling);

        // hit F12
        renderer.Render(ref camera, in option, hittables);

        // setup outputs
        using var diffuseWriter = new StreamWriter("outDiffuse.ppm");
        using var normalWriter = new StreamWriter("outNormal.ppm");
        using var zWriter = new StreamWriter("outZ.ppm");
        framebuffer.ExportToPPM(diffuseWriter, normalWriter, zWriter);
    }
}