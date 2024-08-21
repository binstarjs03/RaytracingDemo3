using System.Diagnostics;

namespace RaytracingDemo;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
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

    public bool Inside(double value)
    {
        return Min < value && value < Max;
    }

    public bool InsideOrEq(double value)
    {
        return Min <= value && value <= Max;
    }

    public override string ToString()
    {
        return $"{Min}, {Max}";
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
