using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Draggable : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Canvas canvas;

    private bool dragging;
    private Vector3 positionOffset;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = FindCanvasWithDraggable();
    }

    void Update()
    {
        if (dragging && Input.GetMouseButton(0) && GetPosMouseAccordingToCanvas(out Vector3 mousePosition))
            transform.position = mousePosition + new Vector3(positionOffset.x, positionOffset.y, positionOffset.z);

        if (Input.GetMouseButtonUp(0))
            dragging = false;
    }

    public bool GetPosMouseAccordingToCanvas(out Vector3 pos)
    {
        if (canvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            pos = Input.mousePosition;
            return true;
        }

        canvas = FindCanvasWithDraggable();
        if (canvas == null)
        {
            pos = Vector3.zero;
            return false;
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = canvas.transform.position.z;
        pos = mainCamera.ScreenToWorldPoint(mousePos);
        return true;
    }

    private Canvas FindCanvasWithDraggable()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        return canvases.FirstOrDefault(e => e.GetComponentInChildren<Draggable>().gameObject.name == name);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Offset to calculate correctly the position where the mouse clicked
        if (GetPosMouseAccordingToCanvas(out Vector3 mousePosition))
            positionOffset = transform.position - mousePosition;


        dragging = true;
    }
}
