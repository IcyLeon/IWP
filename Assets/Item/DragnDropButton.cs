using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;

public class DragnDropButton : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform parentTransform;
    public delegate void OnBeginDragEvent(PointerEventData eventData, Transform parentTransform);
    public OnBeginDragEvent onBeginDragEvent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDragEvent?.Invoke(eventData, parentTransform);
    }

    public delegate void OnDragEvent(PointerEventData eventData);
    public OnDragEvent onDragEvent;
    public void OnDrag(PointerEventData eventData)
    {
        onDragEvent?.Invoke(eventData);
    }

    public OnDragEvent onEndDragEvent;
    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDragEvent?.Invoke(eventData);
    }
}
