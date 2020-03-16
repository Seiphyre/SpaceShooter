using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEntity : MonoBehaviour
{
    // ----- [ Attributes ] ---------------------------------

    [Header("Entity")]

    [SerializeField]
    private bool _shouldDisplayBounds = false;

    protected Bounds _boundsRenderer;

    protected Vector3 _centerDelta;



    // ----- [Functions ] -----------------------------------

    // --v-- Unity Messages --v--

    protected virtual void Awake()
    {
        //_collider = GetComponentInChildren<Collider>();
        _boundsRenderer = GetComponentInChildren<Renderer>().bounds;
        _centerDelta = transform.position - _boundsRenderer.center;
        /*Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            _bounds.Encapsulate(renderer.bounds);
        }*/
    }

    public abstract void Reset();

    // --v-- Boundary --v--

    public virtual MinMaxBounds GetBoundaryPositionIn()
    {
        MinMaxBounds boundaryEntityIn = new MinMaxBounds();
        MinMaxBounds BoundaryMapIn = GameManager.Instance.MapInfo.BoundaryIn;

        boundaryEntityIn.Max = new Vector2(BoundaryMapIn.Max.x - _boundsRenderer.extents.x, BoundaryMapIn.Max.y - _boundsRenderer.extents.y);
        boundaryEntityIn.Min = new Vector2(BoundaryMapIn.Min.x + _boundsRenderer.extents.x, BoundaryMapIn.Min.y + _boundsRenderer.extents.y);

        return boundaryEntityIn;
    }

    public virtual MinMaxBounds GetBoundaryPositionOut()
    {
        MinMaxBounds boundaryEntityOut = new MinMaxBounds();
        MinMaxBounds BoundaryMapOut = GameManager.Instance.MapInfo.BoundaryOut;

        boundaryEntityOut.Max = new Vector2(BoundaryMapOut.Max.x + _boundsRenderer.extents.x, BoundaryMapOut.Max.y + _boundsRenderer.extents.y);
        boundaryEntityOut.Min = new Vector2(BoundaryMapOut.Min.x - _boundsRenderer.extents.x, BoundaryMapOut.Min.y - _boundsRenderer.extents.y);

        return boundaryEntityOut;
    }

    public bool IsOutOfBoundary(BoundaryType BoundaryType)
    {

        MinMaxBounds bulletBoundary = null;

        switch (BoundaryType)
        {
            case BoundaryType.IN:
                bulletBoundary = GetBoundaryPositionIn();
                break;
            case BoundaryType.OUT:
                bulletBoundary = GetBoundaryPositionOut();
                break;
            default:
                bulletBoundary = GetBoundaryPositionIn();
                Debug.LogWarning("[AEntity] BoundaryType switch is default case.");
                break;
        }

        if (!Utils.IsValueInRange(transform.position.x, bulletBoundary.Min.x, bulletBoundary.Max.x))
        {
            // Out of bound on the Horizontal axis (X)
            return true;
        }

        if (!Utils.IsValueInRange(transform.position.y, bulletBoundary.Min.y, bulletBoundary.Max.y))
        {
            // Out of bound on the Vertical axis (Y)
            return true;
        }



        return false;
    }

    // --v-- Size --v--

    public Vector2 GetSize()
    {
        return (new Vector2(_boundsRenderer.size.x, _boundsRenderer.size.y));
    }

    public MinMaxBounds GetExtents()
    {
        Vector2 max = -_centerDelta + new Vector3(_boundsRenderer.extents.x, _boundsRenderer.extents.y, 0f);
        Vector2 min = -_centerDelta + new Vector3(-_boundsRenderer.extents.x, -_boundsRenderer.extents.y, 0f);

        MinMaxBounds extents = new MinMaxBounds(min, max);

        return extents;
    }

    public void OnDrawGizmos()
    {
        if (_shouldDisplayBounds)
        {

            Vector3 a = transform.position + new Vector3(GetExtents().Max.x, GetExtents().Max.y, 0f);
            Vector3 b = transform.position + new Vector3(GetExtents().Max.x, GetExtents().Min.y, 0f);
            Vector3 c = transform.position + new Vector3(GetExtents().Min.x, GetExtents().Min.y, 0f);
            Vector3 d = transform.position + new Vector3(GetExtents().Min.x, GetExtents().Max.y, 0f);

            /*Vector3 a = transform.position - (_centerDelta) + new Vector3(_boundsRenderer.extents.x, _boundsRenderer.extents.y, 0f);
            Vector3 b = transform.position - (_centerDelta) + new Vector3(_boundsRenderer.extents.x, -_boundsRenderer.extents.y, 0f);
            Vector3 c = transform.position - (_centerDelta) + new Vector3(-_boundsRenderer.extents.x, -_boundsRenderer.extents.y, 0f);
            Vector3 d = transform.position - (_centerDelta) + new Vector3(-_boundsRenderer.extents.x, _boundsRenderer.extents.y, 0f);*/

            Gizmos.color = Color.red;

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }
    }
}
