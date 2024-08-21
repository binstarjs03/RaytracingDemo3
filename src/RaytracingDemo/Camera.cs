namespace RaytracingDemo;

public struct Camera(Framebuffer framebuffer, double fieldOfView, in Transformation transformation)
{
    public Framebuffer Framebuffer = framebuffer;
    public double FieldOfView = fieldOfView;
    public Transformation Transformation = transformation;
    
    public readonly int RasterWidth => Framebuffer.Width;
    public readonly int RasterHeight => Framebuffer.Height;
}
