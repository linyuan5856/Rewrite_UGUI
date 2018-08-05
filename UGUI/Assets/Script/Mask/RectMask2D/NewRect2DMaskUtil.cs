using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReWriteUGUI
{
    public static class NewRect2DMaskUtil
    {
        public static void Notify2DMaskStateChanged(Component mask)
        {
            var components = NewListPool<Component>.Get();

            mask.GetComponentsInChildren(components);
            for (var i = 0; i < components.Count; i++)
            {
                if (components[i] == null || components[i].gameObject == mask.gameObject)
                    continue;

                var toNotify = components[i] as IClippable;
                if (toNotify != null)
                    toNotify.RecalculateClipping();
            }

            NewListPool<Component>.Release(components);
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


        public static NewRectMask2D GetRectMaskForClippable(IClippable clippable)
        {
            var rectMaskComponents = NewListPool<NewRectMask2D>.Get();
            var canvasComponents = NewListPool<Canvas>.Get();
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
                    if (canvasComponents[j].overrideSorting &&
                        !IsDesendantOrSelf(canvasComponents[j].transform, targetMask.transform))
                    {
                        targetMask = null;
                        break;
                    }
                }

                return targetMask;
            }

            NewListPool<NewRectMask2D>.Release(rectMaskComponents);
            NewListPool<Canvas>.Release(canvasComponents);

            return targetMask;
        }


        public static void GetRectMasksForClip(NewRectMask2D clipper, List<NewRectMask2D> masks)
        {
            masks.Clear();

            var rectMaskComponents = NewListPool<NewRectMask2D>.Get();
            var canvasComponents = NewListPool<Canvas>.Get();
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


            NewListPool<NewRectMask2D>.Release(rectMaskComponents);
            NewListPool<Canvas>.Release(canvasComponents);
        }
    }
}