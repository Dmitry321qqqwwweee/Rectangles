using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Field : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Rectangle _rectanglePrefab = default;
    [SerializeField]
    private List<Rectangle> _rectangles = new List<Rectangle>();

    [SerializeField]
    private Link _linkPrefab = default;
    private List<Link> _links = new List<Link>();

    private Rectangle _selectRectangle = null;

    public void SelectRect(Rectangle rectangle)
    {
        if (_selectRectangle == null)
        {
            _selectRectangle = rectangle;
        }
        else if (_selectRectangle == rectangle)
        {
            _selectRectangle = null;
        }
        else
        {
            CreateLink(new Rectangle[] { _selectRectangle, rectangle });

            _selectRectangle = null;
        }
    }

    public bool IsPlaceFreeForRectangle(Rectangle rectangle)
    {
        Vector2 _rectanglePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (Rectangle rect in _rectangles)
        {
            Vector2 _rectPosition = rect.GetPosition();
            if ((rect != rectangle) &&
                (Mathf.Abs(_rectPosition.x - _rectanglePosition.x) < rectangle.Size.x + rect.Size.x) &&
                (Mathf.Abs(_rectPosition.y - _rectanglePosition.y) < rectangle.Size.y + rect.Size.y))
            {
                return false;
            }
        }

        return true;
    }

    public Vector2 GetFreeRectanglePosition(Rectangle rectangle)
    {
        Vector2 _rectanglePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (Rectangle rect in _rectangles)
        {
            Vector2 _rectPosition = rect.GetPosition();
            if ((rect != rectangle) &&
                (Mathf.Abs(_rectPosition.x - _rectanglePosition.x) < rect.Size.x * 2) &&
                (Mathf.Abs(_rectPosition.y - _rectanglePosition.y) < rect.Size.y * 2))
            {
                if ((rect.Size.x - Mathf.Abs(_rectanglePosition.x - _rectPosition.x)) <
                    rect.Size.y - Mathf.Abs(_rectanglePosition.y - _rectPosition.y))
                {
                    return new Vector2(
                        _rectPosition.x + rect.Size.x * 2 * (_rectanglePosition.x > _rectPosition.x ? 1 : -1),
                        _rectanglePosition.y
                        );
                }
                else
                {
                    return new Vector2(
                        _rectanglePosition.x,
                        _rectPosition.y + rect.Size.y * 2 * (_rectanglePosition.y > _rectPosition.y ? 1 : -1)
                        );
                }
            }
        }

        return _rectanglePosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CreateRectangle(eventData);
    }

    private void CreateLink(Rectangle[] rectangles)
    {
        foreach (Link link in _links)
        {
            if (link.IsExist(rectangles))
            {
                DestroyLink(link);

                return;
            }
        }

        _links.Add(Instantiate(_linkPrefab, transform).Create(rectangles));
    }

    private void DestroyLink(Link link)
    {
        _links.Remove(link);
        Destroy(link.gameObject);
    }

    private void CreateRectangle(PointerEventData eventData)
    {
        Rectangle _rectangle = Instantiate(_rectanglePrefab, transform).Create(eventData.position);

        if (_rectangle != null)
        {
            _rectangles.Add(_rectangle);
        }
    }

    public void DestroyRectangle(Rectangle rectangle)
    {
        DestroyLinksByRectangle(rectangle);

        _rectangles.Remove(rectangle);
    }

    private void DestroyLinksByRectangle(Rectangle rectangle)
    {
        foreach (Link link in _links)
        {
            if (link.IsExist(rectangle))
            {
                DestroyLink(link);
                DestroyLinksByRectangle(rectangle);

                return;
            }
        }
    }
}
