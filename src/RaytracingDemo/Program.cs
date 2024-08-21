using System.IO;
namespace RaytracingDemo;

class Program
{
    static void Main(string[] args)
    {
        using var writer = new StreamWriter("output.ppm");
        var renderer = new Renderer();
        var camera = new Camera(512, 512, 90, Transformation.Default);
        renderer.Render(writer, ref camera);
    }
}