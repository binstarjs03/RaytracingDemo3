using System;

namespace RaytracingDemo;

public static class Common
{
    public static double ToRadian(this double degree)
    {
        return degree * Math.PI / 180;
    }

    public static readonly double Epsilon = 0.001;
}