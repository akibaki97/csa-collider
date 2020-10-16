using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Complex
{
    public Complex(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Complex(float angle)
    {
        x = Mathf.Cos(angle);
        y = Mathf.Sin(angle);
    }

    public static Complex operator +(Complex lhs, Complex rhs)
    {
        return new Complex(lhs.x + rhs.x, lhs.y + rhs.y);
    }

    public static Complex operator -(Complex lhs, Complex rhs)
    {
        return new Complex(lhs.x - rhs.x, lhs.y - rhs.y);
    }

    public static Complex operator*(Complex lhs, Complex rhs)
    {
        return new Complex(lhs.x * rhs.x - lhs.y * rhs.y, lhs.y * rhs.x + lhs.x * rhs.y);
    }

    public static Vector2 operator*(Complex lhs, Vector2 rhs)
    {
        return new Vector2(lhs.x * rhs.x - lhs.y * rhs.y, lhs.y * rhs.x + lhs.x * rhs.y);
    }

    public static Complex operator*(float lhs, Complex rhs)
    {
        return new Complex(lhs * rhs.x, lhs * rhs.y);
    }

    public static Complex Rotate(float angle)
    {
        return new Complex(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public void Add(Complex rhs)
    {
        x += rhs.x;
        y += rhs.y;
    }

    public void Sub(Complex rhs)
    {
        x -= rhs.x;
        y -= rhs.y;
    }

    public void Mul(Complex rhs)
    {
        float X = x * rhs.x - y * rhs.y;
        float Y = y * rhs.x + x * rhs.y;
        x = X; y = Y;
    }

    public void Div(Complex rhs)
    {
        float sqAbsRhs = rhs.SqAbs;
        float X = (x * rhs.x + y * rhs.y) / sqAbsRhs;
        float Y = (y * rhs.x - x * rhs.y) / sqAbsRhs;
        x = X; y = Y;
    }

    public void Rec()
    {
        float sqAbs = x * x + y * y;
        x = x / sqAbs;
        y = -y / sqAbs;
    }

    public static Complex realUnit = new Complex(1, 0);
    public static Complex imagUnit = new Complex(0, 1);

    public static Complex Conjugated(Complex c) => new Complex(c.x, -c.y);

    public void Set(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static explicit operator Vector2(Complex c) => new Vector2(c.x,c.y);
    public static explicit operator Complex(Vector2 v) => new Complex(v.x,v.y);

    public override string ToString()
    {
        return x.ToString() + " + " + y.ToString() + "i";
    }

    public float Abs { get => Mathf.Sqrt(x * x + y * y); }
    public float SqAbs { get => x * x + y * y; }

    public float Arg { get => Mathf.Atan2(y, x); }

    public float x;
    public float y;
}
