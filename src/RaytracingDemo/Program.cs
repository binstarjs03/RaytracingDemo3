using System.Collections.Generic;
using System.IO;
namespace RaytracingDemo;

class Program
{

    static void Main(string[] args)
    {
        // setup outputs
        using var composite = new StreamWriter("outComposite.ppm");
        using var normal = new StreamWriter("outNormal.ppm");

        // setup render config
        var stream = new RenderStream(composite, normal);
        var renderer = new Renderer();
        var camera = new Camera(512, 512, 90, Transformation.Default);

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

        // hit F12
        renderer.Render(in stream, ref camera, hittables);
    }
}