using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class Rectangle : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler
{
    private const float CLICK_DELAY_TIME = .2f;

    public Action<Rectangle> OnSetPosition = (position) => { };

    [SerializeField]
    private Image _backgroundImage = default;

    private Field _field = default;

    private bool _isClicked = false;
    private bool _isDraged = false;

    public void Create(Vector2 position)
    {
        SetPosition(position);

        _backgroundImage.color = GetRandomColor();
    }

    public Vector3 GetPosition() => transform.position;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetPosition(Input.mousePosition);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isDraged)
        {
            _isDraged = false;
            return;
        }

        if (_isClicked)
        {
            Destroy(gameObject);
        }
        else
        {
            _field.SelectRect(this);
            _isClicked = true;
            Invoke(nameof(ClickDelay), CLICK_DELAY_TIME);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDraged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetPosition(eventData.position);
    }

    private void ClickDelay()
    {
        _isClicked = false;
    }

    private void SetPosition(Vector2 position)
    {
        transform.position = Camera.main.ScreenToWorldPoint(position);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

        OnSetPosition(this);
    }

    private Color GetRandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    private void Awake()
    {
        _field = GetComponentInParent<Field>();
    }

    private void OnDestroy()
    {
        _field.DestroyLinksByRectangle(this);
    }
}
