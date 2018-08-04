using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReWriteUGUI
{
    public static class NewMaskUtil
    {
        public static void Notify2DMaskStateChanged(Component mask)
        {
            var components = new List<Component>();
            mask.GetComponentsInChildren(components);
            for (var i = 0; i < components.Count; i++)
            {
                if (components[i] == null || components[i].gameObject == mask.gameObject)
                    continue;

                var toNotify = components[i] as IClippable;
                if (toNotify != null)
                    toNotify.RecalculateClipping();
            }
        }


        private static bool IsDesendantOrSelf(Transform father, Transform child)
        {
            if (father == null || child == null)
                return false;

            if (father == child)
                return true;

            while (child.parent != null)
            {
                if (child.parent == father)
                    return true;

                child = child.parent;
            }

            return false;
        }


        static readonly List<NewRectMask2D> rectMaskComponents = new List<NewRectMask2D>();
        static readonly List<Canvas> canvasComponents = new List<Canvas>();

        public static NewRectMask2D GetRectMaskForClippable(IClippable clippable)
        {
            rectMaskComponents.Clear();
            canvasComponents.Clear();
            NewRectMask2D targetMask = null;

            clippable.rectTransform.GetComponentsInParent(false, rectMaskComponents);

            for (int i = 0; i < rectMaskComponents.Count; i++)
            {
                targetMask = rectMaskComponents[i];

                if (targetMask.gameObject == clippable.gameObject)
                    continue;
                if (!targetMask.isActiveAndEnabled)
                    continue;

                clippable.rectTransform.GetComponentsInParent(false, canvasComponents);

                for (int j = 0; j < canvasComponents.Count; j++)
                {
                    if (canvasComponents[i].overrideSorting &&
                        !IsDesendantOrSelf(canvasComponents[i].transform, targetMask.transform))
                    {
                        targetMask = null;
                        break;
                    }
                }

                return targetMask;
            }

            return targetMask;
        }


        public static void GetRectMasksForClip(NewRectMask2D clipper, List<NewRectMask2D> masks)
        {
            masks.Clear();

            clipper.GetComponentsInParent(false, rectMaskComponents);

            if (rectMaskComponents.Count > 0)
                clipper.GetComponentsInParent(false, canvasComponents);

            for (int i = rectMaskComponents.Count - 1; i >= 0; i--)
            {
                if (!rectMaskComponents[i].isActiveAndEnabled)
                    continue;

                bool canAdd = true;

                for (int j = canvasComponents.Count - 1; j >= 0; j--)
                {
                    if (canvasComponents[j].overrideSorting && IsDesendantOrSelf(canvasComponents[i].transform,
                            rectMaskComponents[i].transform))
                        canAdd = false;
                }

                if (canAdd)
                    masks.Add(rectMaskComponents[i]);
            }
        }
    }
}