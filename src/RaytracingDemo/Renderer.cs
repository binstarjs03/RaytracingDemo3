using System.IO;

namespace RaytracingDemo;

public interface IRenderer
{
    void Render(StreamWriter writer, ref readonly Camera camera);
}

public class Renderer : IRenderer
{
    public void Render(StreamWriter writer, ref readonly Camera camera)
    {
        writer.WriteLine("P3");
        writer.WriteLine($"{camera.RasterWidth} {camera.RasterHeight}");
        writer.WriteLine("255");
        for (var rasterY = 0; rasterY < camera.RasterHeight; rasterY++)
            for (var rasterX = 0; rasterX < camera.RasterWidth; rasterX++)
            {
                var color = new Vector((double)rasterX / camera.RasterWidth, (double)rasterY / camera.RasterHeight, 0);
                writer.WriteColor(in color);
            }
    }
}