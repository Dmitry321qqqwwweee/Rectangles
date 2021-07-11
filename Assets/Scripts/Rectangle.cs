using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider2D))]
public class Rectangle : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler
{
    private const float CLICK_DELAY_TIME = .2f;

    public Action<Rectangle> OnSetPosition = (position) => { };

    [SerializeField]
    private Image _backgroundImage = default;

    public Vector2 Size { get; private set; } = default;

    private Field _field = default;

    private bool _isClicked = false;
    private bool _isDraged = false;

    public Rectangle Create(Vector2 position)
    {
        Size = GetComponent<Collider2D>().bounds.extents;

        if (!_field.IsPlaceFreeForRectangle(this))
        {
            Destroy(gameObject);

            return null;
        }

        SetPosition(position);

        _backgroundImage.color = GetRandomColor();
  
        return this;
    }

    public Vector3 GetPosition() => transform.position;

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
        SetPosition();
    }

    private void ClickDelay()
    {
        _isClicked = false;
    }

    private void SetPosition(Vector2 position)
    {
        if (!_field.IsPlaceFreeForRectangle(this)) { return; }

        transform.position = Camera.main.ScreenToWorldPoint(position);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

        OnSetPosition(this);
    }

    private void SetPosition()
    {
        transform.position = _field.GetFreeRectanglePosition(this);
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
        _field.DestroyRectangle(this);
    }
}
