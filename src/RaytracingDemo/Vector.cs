using System;
namespace RaytracingDemo;

public readonly struct Vector
{
    public readonly double X;
    public readonly double Y;
    public readonly double Z;

    public Vector(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector Zero => new(0, 0, 0);
    public static Vector Unit => new(1, 1, 1);

    public readonly Vector Normalized => this / Magnitude;
    public readonly double MagnitudeSqr => X * X + Y * Y + Z * Z;
    public readonly double Magnitude => Math.Sqrt(MagnitudeSqr);

    public override string ToString()
    {
        return $"{X}, {Y}, {Z}";
    }

    public static double Dot(in Vector left, in Vector right)
    {
        return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
    }

    public static Vector Cross(in Vector left, in Vector right)
    {
        return new Vector(
            left.Y * right.Z - left.Z * right.Y,
            left.Z * right.X - left.X * right.Z,
            left.X * right.Y - left.Y * right.X);
    }

    public static Vector Reflect(in Vector incoming, in Vector normal)
    {
         return incoming - (normal * Dot(incoming, normal) * 2);
    }

    public static Vector Refract(in Vector incoming, in Vector normal, double iorFrom, double iorTo)
    {
        throw new NotImplementedException();
    }

    // unary
    public static Vector operator -(in Vector self)
    {
        return new Vector(-self.X, -self.Y, -self.Z);
    }

    // binary with scalar
    public static Vector operator +(in Vector self, double scalar)
    {
        return new Vector(self.X + scalar, self.Y + scalar, self.Z + scalar);
    }
    public static Vector operator -(in Vector self, double scalar)
    {
        return new Vector(self.X - scalar, self.Y - scalar, self.Z - scalar);
    }
    public static Vector operator *(in Vector self, double scalar)
    {
        return new Vector(self.X * scalar, self.Y * scalar, self.Z * scalar);
    }
    public static Vector operator /(in Vector self, double scalar)
    {
        return new Vector(self.X / scalar, self.Y / scalar, self.Z / scalar);
    }

    // binary with other vector
    public static Vector operator +(in Vector left, in Vector right)
    {
        return new Vector(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }
    public static Vector operator -(in Vector left, in Vector right)
    {
        return new Vector(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }
    public static Vector operator *(in Vector left, in Vector right)
    {
        return new Vector(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
    }
    public static Vector operator /(in Vector left, in Vector right)
    {
        return new Vector(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
    }

    // conversions
    public static implicit operator Vector(double value)
    {
        return new Vector(value, value, value);
    }
}
