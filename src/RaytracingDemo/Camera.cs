namespace RaytracingDemo;

public struct Camera(int rasterWidth, int rasterHeight, double fieldOfView, in Transformation transformation)
{
    public int RasterWidth = rasterWidth;
    public int RasterHeight = rasterHeight;
    public double FieldOfView = fieldOfView;
    public Transformation Transformation = transformation;
}
