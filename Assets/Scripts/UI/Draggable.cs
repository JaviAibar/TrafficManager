using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Draggable : MonoBehaviour, IPointerDownHandler
{
    private bool dragging;
    private Vector3 positionOffset;

    void Update()
    {
        if (dragging && Input.GetMouseButton(0))
        {
            print("Dragging to " + GetPosMouseAccordingToCanvas() + new Vector3(positionOffset.x, positionOffset.y, positionOffset.z));
            transform.position = GetPosMouseAccordingToCanvas() + new Vector3(positionOffset.x, positionOffset.y, positionOffset.z);
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
    }

    public Vector3 GetPosMouseAccordingToCanvas()
    {
        Canvas canvas = transform.GetComponentInParent<Canvas>();
        print($"Real mouse  {Input.mousePosition}");
        if (canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        // Offset to calculate correctly the position where the mouse clicked
        positionOffset = transform.position - GetPosMouseAccordingToCanvas();
        dragging = true;
    }
}
