using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillEntireScreen : MonoBehaviour
{
    void ResizeSpriteToScreen()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        transform.localScale = new Vector3(1, 1, 1);

        var width = sr.sprite.bounds.size.x;
        var height = sr.sprite.bounds.size.y;

        var worldScreenHeight = Camera.main.orthographicSize * 2.0;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector3(width / (float)worldScreenWidth, height/ (float)worldScreenHeight, 1);
    }

    private void Update()
    {
        ResizeSpriteToScreen();
    }

}
