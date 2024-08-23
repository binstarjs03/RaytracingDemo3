namespace RaytracingDemo;

public class Camera(double fieldOfView, double shiftX, double shiftY, in Transformation transformation)
{
    public double FieldOfView = fieldOfView;
    public double ShiftX = shiftX;
    public double ShiftY = shiftY;
    public Transformation Transformation = transformation;
}
