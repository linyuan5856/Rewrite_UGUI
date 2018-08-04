using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRectangularVertexClipper
{
    private readonly Vector3[] worldCorners = new Vector3[4];
    private readonly Vector3[] canvasCorners = new Vector3[4];

    public Rect GetCanvasRect(RectTransform r, Canvas c)
    {
        if (c == null)
            return new Rect();

        r.GetWorldCorners(worldCorners);
        var canvasRt = c.GetComponent<RectTransform>();
        for (int i = 0; i < 4; i++)
            canvasCorners[i] = canvasRt.InverseTransformPoint(worldCorners[i]);


        return new Rect(canvasCorners[0].x, canvasCorners[0].y, canvasCorners[2].x - canvasCorners[0].x,
            canvasCorners[2].y - canvasCorners[0].y);
    }
}