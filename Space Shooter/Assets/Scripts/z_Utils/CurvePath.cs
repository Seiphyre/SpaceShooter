using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CurvePath : MonoBehaviour
{
    [Header("Drawing")]
    public bool EnablePreview = true;
    public int NbrOfSegement = 50;

    [Header("Path")]

    [SerializeField]
    private List<Transform> Path = new List<Transform>();

    private List<CurveSegment> _curveSegments = new List<CurveSegment>();

    private float _length = 0;


    // -- Methodes -----------------------------------------------------------

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        int i = 0;
        while (i + 1 < Path.Count)
        {
            Transform anchor1 = Path[i];

            // Found next anchor
            int j = i + 1;
            int nextAnchorIndexDist = 1;
            bool anchorFound = false;
            while (j < Path.Count && !anchorFound)
            {
                if (Path[j].tag == "CurveAnchor")
                    anchorFound = true;
                else
                    nextAnchorIndexDist++;
                j++;
            }

            if (anchorFound)
            {
                Transform anchor2 = Path[i + nextAnchorIndexDist];
                float beginDist = _length;
                float endDist = _length;

                if (nextAnchorIndexDist == 2)
                {
                    endDist += Utils.QuadraticBezierLength(anchor1.position, Path[i + 1].position, anchor2.position);

                    _curveSegments.Add(new QuadraticBezierSegment(beginDist, endDist, anchor1, Path[i + 1], anchor2));
                }
                else if (nextAnchorIndexDist == 3)
                {
                    endDist += Utils.CubicBezierLength(anchor1.position, Path[i + 1].position, Path[i + 2].position, anchor2.position);

                    _curveSegments.Add(new CubicBezierSegment(beginDist, endDist, anchor1, Path[i + 1], Path[i + 2], anchor2));
                }
                else
                {
                    endDist += Vector3.Distance(anchor1.position, anchor2.position);

                    _curveSegments.Add(new LinearSegment(beginDist, endDist, anchor1, anchor2));
                }

                i += nextAnchorIndexDist;
                _length = endDist;
            }
            else
                break;
        }
    }

    // --v-- Evaluate Curve --v--

    public Vector3 EvaluatePosition(float tPath)
    {
        float currentDistPath = Mathf.Lerp(0, _length, tPath);

        CurveSegment curveSegment = FindCurveWithDist(currentDistPath);

        float startDistCurve = curveSegment.BeginDist;
        float endDistCurve = curveSegment.EndDist;
        float currentDistCurve = currentDistPath;
        float tCurve;

        endDistCurve -= startDistCurve;
        currentDistCurve -= startDistCurve;

        tCurve = currentDistCurve / endDistCurve;

        Vector3 position = curveSegment.EvaluatePosition(tCurve);

        return position;
    }

    public Quaternion EvaluateRotation(float tPath)
    {
        float currentDistPath = Mathf.Lerp(0, _length, tPath);

        CurveSegment curveSegment = FindCurveWithDist(currentDistPath);

        float startDistCurve = curveSegment.BeginDist;
        float endDistCurve = curveSegment.EndDist;
        float currentDistCurve = currentDistPath;
        float tCurve;

        endDistCurve -= startDistCurve;
        currentDistCurve -= startDistCurve;

        tCurve = currentDistCurve / endDistCurve;

        Quaternion rotation = curveSegment.EvaluateRotation(tCurve);

        return rotation;
    }

    private CurveSegment FindCurveWithDist(float dist)
    {
        return _curveSegments.Find(curve => curve.BeginDist <= dist && curve.EndDist >= dist);
    }

    public float Length { get { return _length; } }

    // --v-- Draw Gizmos --v--

    private void OnDrawGizmos()
    {
        if (EnablePreview && Path != null && Path.Count >= 2)
        {

            /*int i = 0;

            while (i + 2 < Path.Count)
            {
                Vector3 anchor1 = Path[i].position;
                Vector3 control = Path[i + 1].position;
                Vector3 anchor2 = Path[i + 2].position;

                Gizmos.color = Color.red;

                Gizmos.DrawLine(anchor1, control);
                Gizmos.DrawLine(control, anchor2);

                float t = 0;

                Gizmos.color = Color.blue;

                while (t < 1)
                {
                    Vector3 startPoint = Utils.QuadraticBezier(t, anchor1, control, anchor2);
                    t += 1.0f / (float)NbrOfSegement;
                    Vector3 endPoint = Utils.QuadraticBezier(t, anchor1, control, anchor2);

                    Gizmos.DrawLine(startPoint, endPoint);
                }

                i += 2;
            }*/

            int i = 0;
            while (i + 1 < Path.Count)
            {
                Vector3 anchor1 = Path[i].position;

                // Found next anchor
                int j = i + 1;
                int nextAnchorIndexDist = 1;
                bool anchorFound = false;
                while (j < Path.Count && !anchorFound)
                {
                    if (Path[j].tag == "CurveAnchor")
                        anchorFound = true;
                    else
                        nextAnchorIndexDist++;
                    j++;
                }

                if (anchorFound)
                {
                    Vector3 anchor2 = Path[i + nextAnchorIndexDist].position;
                    if (nextAnchorIndexDist == 2)
                        DrawQuadraticBezier(anchor1, Path[i + 1].position, anchor2);
                    else if (nextAnchorIndexDist == 3)
                        DrawCubicBezier(anchor1, Path[i + 1].position, Path[i + 2].position, anchor2);
                    else
                        DrawLerp(anchor1, anchor2);

                    i += nextAnchorIndexDist;
                }
                else
                    break;
            }

            i = 0;
            Gizmos.color = Color.red;
            while (i < Path.Count)
            {
                if (Path[i].tag == "CurveAnchor")
                    Gizmos.DrawSphere(Path[i].position, 0.2f);
                else
                    Gizmos.DrawSphere(Path[i].position, 0.1f);
                i++;
            }
        }
    }

    private void DrawLerp(Vector3 anchor1, Vector3 anchor2)
    {
        float t = 0;

        while (t < 1)
        {
            Vector3 startPoint = Vector3.Lerp(anchor1, anchor2, t);
            t += 1.0f / (float)NbrOfSegement;
            Vector3 endPoint = Vector3.Lerp(anchor1, anchor2, t);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }

    private void DrawQuadraticBezier(Vector3 anchor1, Vector3 control, Vector3 anchor2)
    {
        float t = 0;

        while (t < 1)
        {
            Vector3 startPoint = Utils.QuadraticBezier(t, anchor1, control, anchor2);
            t += 1.0f / (float)NbrOfSegement;
            Vector3 endPoint = Utils.QuadraticBezier(t, anchor1, control, anchor2);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(startPoint, endPoint);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawLine(anchor1, control);
        Gizmos.DrawLine(control, anchor2);
    }

    private void DrawCubicBezier(Vector3 anchor1, Vector3 control1, Vector3 control2, Vector3 anchor2)
    {
        float t = 0;

        while (t < 1)
        {
            Vector3 startPoint = Utils.CubicBezier(t, anchor1, control1, control2, anchor2);
            t += 1.0f / (float)NbrOfSegement;
            Vector3 endPoint = Utils.CubicBezier(t, anchor1, control1, control2, anchor2);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPoint, endPoint);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawLine(anchor1, control1);
        Gizmos.DrawLine(control2, anchor2);
    }

    /*public float PathLength(int segment = 20)
    {
        int i = 0;
        float dist = 0;

        while (i + 2 < Path.Count)
        {
            Vector3 anchor1 = Path[i].position;
            Vector3 control = Path[i + 1].position;
            Vector3 anchor2 = Path[i + 2].position;

            float t = 0;

            while (t < 1)
            {
                Vector3 startPoint = Utils.QuadraticBezier(t, anchor1, control, anchor2);
                t += 1.0f / (float)segment;
                Vector3 endPoint = Utils.QuadraticBezier(t, anchor1, control, anchor2);

                dist += Vector3.Distance(startPoint, endPoint);
            }

            i += 2;
        }

        return dist;
    }*/

}

public abstract class CurveSegment
{
    protected float _beginDist;
    protected float _endDist;

    protected Transform _anchor1;
    protected Transform _anchor2;

    protected CurveSegment(float beginDist, float endDist, Transform anchor1, Transform anchor2)
    {
        _beginDist = beginDist;
        _endDist = endDist;
        _anchor1 = anchor1;
        _anchor2 = anchor2;
    }

    public abstract Vector3 EvaluatePosition(float t);
    public abstract Quaternion EvaluateRotation(float t);

    public float BeginDist { get { return _beginDist; } }
    public float EndDist { get { return _endDist; } }
}

public class LinearSegment : CurveSegment
{

    public LinearSegment(float beginDist, float endDist, Transform anchor1, Transform anchor2) : base(beginDist, endDist, anchor1, anchor2) { }

    public override Vector3 EvaluatePosition(float t)
    {
        return Vector3.Lerp(_anchor1.position, _anchor2.position, t);
    }

    public override Quaternion EvaluateRotation(float t)
    {
        return Quaternion.Lerp(_anchor1.rotation, _anchor2.rotation, t);
    }
}

public class QuadraticBezierSegment : CurveSegment
{
    private Transform _control1;

    public QuadraticBezierSegment(float beginDist, float endDist, Transform anchor1, Transform control1, Transform anchor2) : base(beginDist, endDist, anchor1, anchor2)
    {
        _control1 = control1;
    }

    public override Vector3 EvaluatePosition(float t)
    {
        return Utils.QuadraticBezier(t, _anchor1.position, _control1.position, _anchor2.position);
    }

    public override Quaternion EvaluateRotation(float t)
    {
        return Utils.QuadraticBezier(t, _anchor1.rotation, _control1.rotation, _anchor2.rotation);
    }
}

public class CubicBezierSegment : CurveSegment
{
    private Transform _control1;
    private Transform _control2;

    public CubicBezierSegment(float beginDist, float endDist, Transform anchor1, Transform control1, Transform control2, Transform anchor2) : base(beginDist, endDist, anchor1, anchor2)
    {
        _control1 = control1;
        _control2 = control2;
    }

    public override Vector3 EvaluatePosition(float t)
    {
        return Utils.CubicBezier(t, _anchor1.position, _control1.position, _control2.position, _anchor2.position);
    }

    public override Quaternion EvaluateRotation(float t)
    {
        return Utils.CubicBezier(t, _anchor1.rotation, _control1.rotation, _control2.rotation, _anchor2.rotation);
    }

}
