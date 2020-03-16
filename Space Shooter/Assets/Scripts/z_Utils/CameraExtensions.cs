using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtensions
{
    public static float GetHeight(this Camera cam)
    {
        float camHeight = 2f * cam.orthographicSize;

        return camHeight;
    }

    public static float GetWidth(this Camera cam)
    {
        float camWidth = 2f * cam.orthographicSize * cam.aspect;

        return camWidth;
    }

    public static Vector3 GetMaxBoundary(this Camera cam)
    {
        float camHeight = cam.GetHeight();
        float camWidth = cam.GetWidth();

        Vector3 camMaxBoundary = cam.transform.position + new Vector3((camWidth / 2f), (camHeight / 2f), 0);

        return camMaxBoundary;
    }

    public static Vector3 GetMinBoundary(this Camera cam)
    {
        float camHeight = cam.GetHeight();
        float camWidth = cam.GetWidth();

        Vector3 camMinBoundary = cam.transform.position - new Vector3((camWidth / 2f), (camHeight / 2f), 0);

        return camMinBoundary;
    }

    public static MinMaxBounds GetBoundary(this Camera cam)
    {
        MinMaxBounds boundary = new MinMaxBounds();

        boundary.Min = cam.GetMinBoundary();
        boundary.Max = cam.GetMaxBoundary();

        return boundary;
    }
}