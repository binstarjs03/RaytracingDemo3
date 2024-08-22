namespace RaytracingDemo;

public class Camera(double fieldOfView, in Transformation transformation)
{
    public double FieldOfView = fieldOfView;
    public Transformation Transformation = transformation;
}
