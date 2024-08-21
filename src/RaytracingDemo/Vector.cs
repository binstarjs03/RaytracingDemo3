using System;
using System.Diagnostics.CodeAnalysis;
namespace RaytracingDemo;

public readonly struct Vector : IEquatable<Vector>
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

    public override string ToString()
    {
        return $"{X}, {Y}, {Z}";
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Vector vector && Equals(vector);
    }

    public bool Equals(Vector other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public static bool operator ==(Vector left, Vector right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(Vector left, Vector right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());
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

public static class VectorUtils
{
    public static Vector NextVectorOnHemisphere(this Random random, in Vector normal)
    {
        var theta = Math.Acos(1 - random.NextDouble()); // random longitude
        var phi = Math.PI * 2 * random.NextDouble(); // random latitude
        var x = Math.Sin(theta) * Math.Cos(phi);
        var y = Math.Cos(theta);
        var z = Math.Sin(theta) * Math.Sin(phi);
        var n = normal;
        var up = new Vector(0, 1, 0);
        var right = new Vector(1, 0, 0);
        var t = Vector.Cross(in n, n != up ? up : right);
        var b = Vector.Cross(in n, in t);
        var result = t * x + y * n + z * b;
        return result;
    }
}