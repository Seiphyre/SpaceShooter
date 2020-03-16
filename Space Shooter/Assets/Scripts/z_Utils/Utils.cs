using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static bool IsValueInRange(float value, float min, float max)
    {
        if (Mathf.Clamp(value, min, max) != value)
            return false;

        return true;
    }

    public static Vector3 QuadraticBezier(float t, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;

        Vector3 p = uu * p1;
        p += 2 * u * t * p2;
        p += tt * p3;

        return p;
    }

    public static Quaternion QuadraticBezier(float t, Quaternion a, Quaternion b, Quaternion c)
    {
        // Quaternion p0 = Quaternion.Slerp(a, b, t);
        // Quaternion p1 = Quaternion.Slerp(b, c, t);

        // return Quaternion.Slerp(p0, p1, t);
        return Quaternion.Slerp(a, c, t);
    }

    public static float QuadraticBezierLength(Vector3 p1, Vector3 p2, Vector3 p3, int segment = 20)
    {
        float dist = 0;
        float t = 0;

        while (t < 1)
        {
            Vector3 startPoint = Utils.QuadraticBezier(t, p1, p2, p3);
            t += 1.0f / (float)segment;
            Vector3 endPoint = Utils.QuadraticBezier(t, p1, p2, p3);

            dist += Vector3.Distance(startPoint, endPoint);
        }

        return dist;
    }

    public static Vector3 CubicBezier(float t, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        float u = 1 - t;
        float uu = u * u;
        float uuu = uu * u;
        float tt = t * t;
        float ttt = tt * t;

        Vector3 p = uuu * p1;
        p += 3 * uu * t * p2;
        p += 3 * u * tt * p3;
        p += ttt * p4;

        return p;
    }

    public static Quaternion CubicBezier(float t, Quaternion a, Quaternion b, Quaternion c, Quaternion d)
    {
        // Quaternion p0 = QuadraticBezier(t, a, b, c);
        // Quaternion p1 = QuadraticBezier(t, b, c, d);

        // return Quaternion.Slerp(p0, p1, t);
        return Quaternion.Slerp(a, d, t);
    }

    public static float CubicBezierLength(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int segment = 20)
    {
        float dist = 0;
        float t = 0;

        while (t < 1)
        {
            Vector3 startPoint = Utils.CubicBezier(t, p1, p2, p3, p4);
            t += 1.0f / (float)segment;
            Vector3 endPoint = Utils.CubicBezier(t, p1, p2, p3, p4);

            dist += Vector3.Distance(startPoint, endPoint);
        }

        return dist;
    }
}

public class MinMaxBounds
{
    public Vector2 Min;
    public Vector2 Max;

    public MinMaxBounds()
    {
        Min = new Vector2();
        Max = new Vector2();
    }
    public MinMaxBounds(Vector2 min, Vector2 max)
    {
        Min = min;
        Max = max;
    }
}

public enum BoundaryType
{
    IN,
    OUT
}