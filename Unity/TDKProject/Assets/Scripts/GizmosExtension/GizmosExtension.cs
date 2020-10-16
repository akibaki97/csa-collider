using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;

/// <summary>
/// Extending the Gizmos with various shape (circle, polygon, rectangle) drawing
/// </summary>
public static class GizmosExtension
{
    public static Color lightGreen = new Color(141f / 256f, 235f / 256f, 136f / 256f);
    public static Color lightRed = new Color(235f / 256f, 136f / 256f, 141f / 256f);
    public static Color lightBlue = new Color(136f / 256f, 141f / 256f, 235f / 256f);

    public static void DrawCircle(Vector2 center, float Radius, Vector3 localX, Vector3 localY, Vector3 refPoint)
    {
        int n = 50;
        float stride = 2 * Mathf.PI / n;
        for (int i = 0; i < n; i++)
        {
            Vector3 vec1 = (Radius * Mathf.Cos(stride * i) + center.x)*localX + (Radius * Mathf.Sin(stride * i) + center.y)*localY + refPoint;
            Vector3 vec2 = (Radius * Mathf.Cos(stride * (i+1)) + center.x)*localX + (Radius * Mathf.Sin(stride * (i+1)) + center.y)*localY + refPoint;

            Gizmos.DrawLine(vec1, vec2);
        }
    }

    public static void DrawRectangle(Vector2[] vertices, Vector3 localX, Vector3 localY, Vector3 refPoint, Color col)
    {
        Gizmos.color = col;
        if(vertices.Length == 4)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 vec1 = vertices[i].x * localX + vertices[i].y * localY + refPoint;
                Vector3 vec2 = vertices[(i+1)%4].x * localX + vertices[(i+1)%4].y * localY + refPoint;

                Gizmos.DrawLine(vec1, vec2);
            }
        }
    }

    public static void DrawPolygon(Polygon polygon, Vector3 localX, Vector3 localY, Vector3 refPoint, Color col)
    {
        Gizmos.color = col;
        for (int i = 0; i < polygon.Count - 1; i++)
        {
            Vector3 vec1 = (localX * polygon[i].x + localY * polygon[i].y) + refPoint;
            Vector3 vec2 = (localX * polygon[i + 1].x + localY * polygon[i + 1].y) + refPoint;
            Gizmos.DrawLine(vec1, vec2);
        }
    }

    public static void DrawCurve(FourierCurve curve, Vector3 localX, Vector3 localY, Vector3 refPoint, Color col)
    {
        int n = 100;
        Gizmos.color = col;
        float stride = 2 * Mathf.PI / n;
        for (int t = 0; t < n; t++)
        {
            Vector3 vec1 = curve.EvalX(stride * t) * localX + curve.EvalY(stride * t) * localY + refPoint;
            Vector3 vec2 = curve.EvalX(stride * (t +1)) * localX + curve.EvalY(stride * (t + 1)) * localY + refPoint;
            Gizmos.DrawLine(vec1, vec2);
        }
    }
}
