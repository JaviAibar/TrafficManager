using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioController : MonoBehaviour
{
    AspectRatioFitter aspectRatio;
    Image background;
    private void Awake()
    {
        aspectRatio = GetComponent<AspectRatioFitter>();
        background = GetComponent<Image>();
    }

    void Start()
    {
        float width = background.sprite.texture.width;
        float height = background.sprite.texture.height;
        float ratio = width > height ? width / height : height / width;
        aspectRatio.aspectRatio = ratio;
    }
}
