using System;
namespace RaytracingDemo;

class Program
{
    static void Main(string[] args)
    {
        var incoming = new Vector(1, 1, 0);
        var normal = new Vector(0, 1, 0);
        var reflected = Vector.Reflect(in incoming, in normal);

        Console.WriteLine(reflected);
    }
}