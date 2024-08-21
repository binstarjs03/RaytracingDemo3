using System;
namespace RaytracingDemo;

class Program
{
    static void Main(string[] args)
    {
        // using var writer = new StreamWriter("output.ppm");
        // var renderer = new Renderer();
        // var camera = new Camera(512, 512, 90, Transformation.Default);
        // renderer.Render(writer, ref camera);
        var random = new Random(0);
        var normal = new Vector(0, 1, 0);
        for (int i = 0; i < 10; i++)
            Console.WriteLine(random.NextVectorOnHemisphere(normal));
    }
}