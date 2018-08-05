using UnityEngine;
using UnityEngine.UI;

namespace ReWriteUGUI
{
    public class NewMaskUtil
    {
        public static void NotifyStencilStateChanged(Component mask)
        {
            var components = NewListPool<Component>.Get();

            mask.GetComponentsInChildren(components);

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] == null || components[i].gameObject == mask.gameObject)
                    continue;


                var maskable = components[i] as IMaskable;
                if (maskable != null)
                    maskable.RecalculateMasking();
            }

            NewListPool<Component>.Release(components);
        }

        public static Transform FindRootSortOverrideCanvas(Transform start)
        {
            var canvasList = NewListPool<Canvas>.Get();

            start.GetComponentsInParent(false, canvasList);

            Canvas target = null;
            for (int i = 0; i < canvasList.Count; i++)
            {
                target = canvasList[i];
                if (canvasList[i].overrideSorting)
                    break;
            }

            return target != null ? target.transform : null;
        }

        public static int GetStencilDepth(Transform transform, Transform stopAfter)
        {
            var depth = 0;
            if (transform == stopAfter)
                return depth;

            Transform t = transform.parent;

            var maskComponents = NewListPool<NewMask>.Get();
            while (t != null)
            {
                t.GetComponents(maskComponents);

                for (int i = 0; i < maskComponents.Count; i++)
                {
                    var mask = maskComponents[i];
                    if (mask != null && mask.MaskEnabled() && mask.graphic.IsActive())
                    {
                        depth++;
                        break;
                    }
                }

                if (t == stopAfter)
                    break;

                t = t.parent;
            }

            NewListPool<NewMask>.Release(maskComponents);
            return depth;
        }
    }
}