using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Extensions;
using UnityEngine.UI;

[ExecuteAlways]
public class BorderedSelectable : _Selectable
{
    public Selectable[] _graphics;
    public RectTransform combinedRectTrs;

    public virtual void Start ()
    {
        Canvas.ForceUpdateCanvases();
        RectTransform[] children = GetComponentsInChildren<RectTransform>();
        children = children.Remove_class(combinedRectTrs);
        Rect[] graphicRects = new Rect[children.Length];
        for (int i = 0; i < children.Length; i ++)
            graphicRects[i] = children[i].GetRectInCanvasNormalized(canvasRectTrs);
        combinedRectTrs.sizeDelta = RectExtensions.Combine(graphicRects).size.Multiply(canvasRectTrs.sizeDelta);
    }

    public virtual void LateUpdate ()
    {
        for (int i = 0; i < _graphics.Length; i ++)
            _graphics[i].colors = selectable.colors;
    }
}
