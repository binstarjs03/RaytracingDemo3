using System.IO;

namespace RaytracingDemo;

public class Framebuffer(int width, int height)
{
    public readonly int Width = width;
    public readonly int Height = height;
    public readonly Vector[] DiffuseDirect = new Vector[width * height];
    public readonly Vector[] DiffuseIndirect = new Vector[width * height];
    public readonly Vector[] DiffuseAlbedo = new Vector[width * height];
    public readonly Vector[] Normal = new Vector[width * height];
    public readonly Vector[] Z = new Vector[width * height];

    public void ExportToPPM(string postfix)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "output");
        Directory.CreateDirectory(path);
        using var diffuseDirect = new StreamWriter(Path.Combine(path, $"diffuseDirect{postfix}.ppm"));
        using var diffuseIndirect = new StreamWriter(Path.Combine(path, $"diffuseIndirect{postfix}.ppm"));
        using var diffuseAlbedo = new StreamWriter(Path.Combine(path, $"diffuseAlbedo{postfix}.ppm"));
        using var normal = new StreamWriter(Path.Combine(path, $"normal{postfix}.ppm"));
        using var z = new StreamWriter(Path.Combine(path, $"z{postfix}.ppm"));
        using var combined = new StreamWriter(Path.Combine(path, $"combined{postfix}.ppm"));
        WriteHeader(diffuseDirect);
        WriteHeader(diffuseIndirect);
        WriteHeader(diffuseAlbedo);
        WriteHeader(normal);
        WriteHeader(z);
        WriteHeader(combined);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var index = x + y * Width;
                diffuseDirect.WriteColor(DiffuseDirect[index].ToGamma());
                diffuseIndirect.WriteColor(DiffuseIndirect[index].ToGamma());
                diffuseAlbedo.WriteColor(DiffuseAlbedo[index].ToGamma());
                normal.WriteColor(Normal[index].ToGamma());
                z.WriteColor(Z[index].ToGamma());
                var finalColor = (DiffuseDirect[index] + DiffuseIndirect[index]) * DiffuseAlbedo[index];
                combined.WriteColor(finalColor.ToGamma());
            }
    }
    
    private void WriteHeader(StreamWriter writer)
    {
        writer.WriteLine("P3");
        writer.WriteLine($"{Width} {Height}");
        writer.WriteLine("255");
    }
}