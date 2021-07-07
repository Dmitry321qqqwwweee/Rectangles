using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Link : MonoBehaviour
{
    private const int VERTEX_COUNT = 2;
    private const float Z_POSITION = .5f;

    private LineRenderer _lineRenderer = default;

    private List<Rectangle> _rectangles = new List<Rectangle>();

    public Link Create(Rectangle[] rectangles)
    {
        List<Vector3> _positions = new List<Vector3>();

        foreach (Rectangle rectangle in rectangles)
        {
            _rectangles.Add(rectangle);

            rectangle.OnSetPosition += Rectangle_OnSetPosition;

            Vector2 _position = rectangle.GetPosition();
            _positions.Add(new Vector3(_position.x, _position.y, Z_POSITION));
        }

        _lineRenderer.positionCount = VERTEX_COUNT;
        _lineRenderer.SetPositions(_positions.ToArray());

        return this;
    }

    public bool IsExist(Rectangle rectangle)
    {
        foreach (Rectangle _rectangle in _rectangles)
        {
            if (rectangle == _rectangle)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsExist(Rectangle[] rectangles)
    {
        foreach (Rectangle rectangle in _rectangles)
        {
            if (rectangle == rectangles[0])
            {
                foreach (Rectangle rectangle1 in _rectangles)
                {
                    if (rectangle1 == rectangles[1])
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void Rectangle_OnSetPosition(Rectangle rectangle)
    {
        Vector2 _position = rectangle.GetPosition();
        _lineRenderer.SetPosition(_rectangles.IndexOf(rectangle), new Vector3(_position.x, _position.y, Z_POSITION));
    }

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnDestroy()
    {
        foreach (Rectangle rectangle in _rectangles)
        {
            rectangle.OnSetPosition -= Rectangle_OnSetPosition;
        }
    }
}
