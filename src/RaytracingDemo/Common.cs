using System;

namespace RaytracingDemo;

public static class Common
{
    public static double ToRadian(this double degree)
    {
        return degree * Math.PI / 180;
    }

    public static readonly double Bias = 0.001;
}