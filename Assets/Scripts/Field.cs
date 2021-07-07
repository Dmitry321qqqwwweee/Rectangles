using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Field : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Rectangle _rectanglePrefab = default;

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

    public void DestroyLinksByRectangle(Rectangle rectangle)
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Instantiate(_rectanglePrefab, transform).Create(eventData.position);
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
}
