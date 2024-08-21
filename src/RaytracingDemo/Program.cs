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
            new( 0, 1,  -1),
        };
        int[] indices =
        {
            0, 1, 2,
        };
        var hittables = new List<IHittable>()
        {
            new Sphere(new Vector(0,0,-1), 0.5),
            new Sphere(new Vector(0.5,0,-1), 0.5),
            new TriMesh(positions, indices),
        };

        // setup render config
        var width = 16;
        var height = 9;
        var multiplier = 40;
        var framebuffer = new Framebuffer(width * multiplier, height * multiplier);
        var renderer = new Renderer();
        var fov = 90;
        var camera = new Camera(framebuffer, fov, Transformation.Default);

        // hit F12
        renderer.Render(ref camera, hittables);
        
        // setup outputs
        using var diffuseWriter = new StreamWriter("outDiffuse.ppm");
        using var normalWriter = new StreamWriter("outNormal.ppm");
        framebuffer.ExportToPPM(diffuseWriter, normalWriter);
    }
}