using UnityEngine;
using UnityEngine.UI;

public class MyTests : MonoBehaviour
{
    [SerializeField] private float a;
    [SerializeField] private float b;
    [SerializeField] private float t;

    [ContextMenu("Size")]
    public void Size()
    {
        Renderer rend = transform.GetComponent<Renderer>() ?? transform.GetComponentInChildren<Renderer>();
        if (rend)
            print(rend.bounds.size);
        Image im = transform.GetComponent<Image>() ?? transform.GetComponentInChildren<Image>();
        RectTransform rect = im?.GetComponent<RectTransform>();
        Canvas canvas = rect?.GetComponentInParent<Canvas>();
        CanvasScaler canvasScaler = canvas?.GetComponent<CanvasScaler>();
        if (rect && canvas && canvasScaler)
            //return (RectTransformUtility.PixelAdjustRect(rect, canvas).size * (2.54f / Screen.dpi)) / canvasScaler.referencePixelsPerUnit;
            print(rect.rect.size / (canvasScaler.referencePixelsPerUnit / 10));
    }


    [ContextMenu("Lerp")]
    public void PrintLerp()
    {
        print($"Lerp(a:{a}, b:{b}, t:{t})");
        print(Mathf.Lerp(a,b, t));
    }
}
