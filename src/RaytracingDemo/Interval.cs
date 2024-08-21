namespace RaytracingDemo;

public readonly struct Interval(double min, double max)
{
    public readonly double Min = min;
    public readonly double Max = max;

    public double Clamp(double value)
    {
        if (value < Min) return Min;
        if (value > Max) return Max;
        return value;
    }
}
