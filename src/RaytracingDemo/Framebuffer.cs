using System;
using System.IO;

namespace RaytracingDemo;

[Flags]
public enum ExportOption
{
    JustCombined = 0,
    DiffuseDirect = 1 << 0,
    DiffuseIndirect = 1 << 1,
    DiffuseAlbedo = 1 << 2,
    Normal = 1 << 3,
    Z = 1 << 4,
}

public class Framebuffer(int width, int height)
{
    public readonly int Width = width;
    public readonly int Height = height;
    public readonly Vector[] DiffuseDirect = new Vector[width * height];
    public readonly Vector[] DiffuseIndirect = new Vector[width * height];
    public readonly Vector[] DiffuseAlbedo = new Vector[width * height];
    public readonly Vector[] Normal = new Vector[width * height];
    public readonly Vector[] Z = new Vector[width * height];

    public void ExportToPPM(string postfix, ExportOption option = ExportOption.JustCombined)
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        Directory.CreateDirectory(dir);
        using var combined = new StreamWriter(Path.Combine(dir, $"combined{postfix}.ppm"));

        WriteHeader(combined);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var index = x + y * Width;
                var finalColor = (DiffuseDirect[index] + DiffuseIndirect[index]) * DiffuseAlbedo[index];
                combined.WriteColor(finalColor.ToGamma());
            }

        ExportToPPM(dir, "diffuseDirect", postfix, DiffuseDirect, option, ExportOption.DiffuseDirect);
        ExportToPPM(dir, "diffuseIndirect", postfix, DiffuseIndirect, option, ExportOption.DiffuseIndirect);
        ExportToPPM(dir, "diffuseAlbedo", postfix, DiffuseAlbedo, option, ExportOption.DiffuseAlbedo);
        ExportToPPM(dir, "normal", postfix, Normal, option, ExportOption.Normal);
        ExportToPPM(dir, "z", postfix, Z, option, ExportOption.Z);
    }

    private void ExportToPPM(string dir, string name, string postfix, Vector[] buffer, ExportOption option, ExportOption match)
    {
        if ((option & match) != match)
            return;
        using var stream = new StreamWriter(Path.Combine(dir, $"{name}{postfix}.ppm"));
        WriteHeader(stream);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var index = x + y * Width;
                stream.WriteColor(buffer[index].ToGamma());
            }
    }

    private void WriteHeader(StreamWriter writer)
    {
        writer.WriteLine("P3");
        writer.WriteLine($"{Width} {Height}");
        writer.WriteLine("255");
    }
}