using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LY_MaskableGraphic : LY_Graphic, IMaskable, IClippable, IMaterialModifier
{
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RecalculateMasking()
    {
    }

    public void RecalculateClipping()
    {
    }

    public void Cull(Rect clipRect, bool validRect)
    {
    }

    public void SetClipRect(Rect value, bool validRect)
    {
    }

    public RectTransform rectTransform { get; private set; }

    public Material GetModifiedMaterial(Material baseMaterial)
    {
        return null;
    }
}