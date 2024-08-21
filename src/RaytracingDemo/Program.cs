using System.Collections.Generic;
using System.IO;
namespace RaytracingDemo;

class Program
{
    static void Main(string[] args)
    {
        using var composite = new StreamWriter("outComposite.ppm");
        using var normal = new StreamWriter("outNormal.ppm");
        var stream = new RenderStream(composite, normal);
        var renderer = new Renderer();
        var camera = new Camera(512, 512, 90, Transformation.Default);
        var hittables = new List<IHittable>()
        {
            new Sphere(new Vector(0,0,-1), 0.5)
        };
        renderer.Render(in stream, ref camera, hittables);
    }
}