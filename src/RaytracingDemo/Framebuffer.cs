using System.IO;

namespace RaytracingDemo;

public class Framebuffer(int width, int height)
{
    public readonly int Width = width;
    public readonly int Height = height;
    public readonly Vector[] DiffuseBuffer = new Vector[width * height];
    public readonly Vector[] NormalBuffer = new Vector[width * height];
    public readonly Vector[] ZBuffer = new Vector[width * height];

    public void ExportToPPM(StreamWriter diffuseWriter, StreamWriter normalWriter, StreamWriter zWriter)
    {
        WriteHeader(diffuseWriter);
        WriteHeader(normalWriter);
        WriteHeader(zWriter);
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var index = x + y * Width;
                diffuseWriter.WriteColor(DiffuseBuffer[index].ToGamma());
                normalWriter.WriteColor(in NormalBuffer[index]);
                zWriter.WriteColor(in ZBuffer[index]);
            }
    }
    
    private void WriteHeader(StreamWriter writer)
    {
        writer.WriteLine("P3");
        writer.WriteLine($"{Width} {Height}");
        writer.WriteLine("255");
    }
}