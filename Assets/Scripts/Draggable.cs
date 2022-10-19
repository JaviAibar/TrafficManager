using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IPointerDownHandler
{
    private bool dragging;
    private Vector2 positionOffset;

    void Update()
    {
        if (dragging && Input.GetMouseButton(0))
        {
            transform.position = Input.mousePosition + new Vector3(positionOffset.x, positionOffset.y, 0);
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Offset to calculate correctly the position where the mouse clicked
        positionOffset = transform.position - Input.mousePosition;
        dragging = true;
    }
}
