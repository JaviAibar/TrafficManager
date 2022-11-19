using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Draggable : MonoBehaviour, IPointerDownHandler
{
    private bool dragging;
    private Vector3 positionOffset;
    public Canvas canvas;
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
        //Canvas canvas = /*transform.GetComponentInParent<Canvas>();*/ transform.parent.GetComponent<Canvas>();
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        canvas = canvases.FirstOrDefault(e => e.GetComponentInChildren<Draggable>().gameObject.name == name);
        print($"Real mouse  {Input.mousePosition}");
        if (canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return Input.mousePosition;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = /*Camera.main.*/canvas.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

   /* public Vector3 GetPosAccordingToCanvas()
    {
        Canvas canvas = transform.GetComponentInParent<Canvas>();
        print($"Real pos  {transform.position}");
        if (canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return transform.position;
        return Camera.main.ScreenToWorldPoint(transform.position);
    }*/

    public void OnPointerDown(PointerEventData eventData)
    {
        // Offset to calculate correctly the position where the mouse clicked
        positionOffset = /*GetPosAccordingToCanvas()*/ transform.position - GetPosMouseAccordingToCanvas();
        dragging = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
      //  Gizmos.DrawSphere(GetPosMouseAccordingToCanvas(), 2);
        Vector2 size = ((RectTransform)transform).sizeDelta;
        //Gizmos.DrawCube(canvas.transform.position, new Vector3(size.x, size.y, 1));
    }
}
