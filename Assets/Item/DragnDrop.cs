using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragnDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform parentTransform;
    public delegate void OnBeginDragEvent(PointerEventData eventData, Transform parentTransform);
    public event OnBeginDragEvent onBeginDragEvent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDragEvent?.Invoke(eventData, parentTransform);
    }

    public delegate void OnDragEvent(PointerEventData eventData);
    public event OnDragEvent onDragEvent;
    public void OnDrag(PointerEventData eventData)
    {
        onDragEvent?.Invoke(eventData);
    }

    public delegate void OnEndDragEvent(PointerEventData eventData);
    public event OnEndDragEvent onEndDragEvent;
    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDragEvent?.Invoke(eventData);
    }
}
