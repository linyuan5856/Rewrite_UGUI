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
        }


        private static bool IsDesendantOrSelf(Transform father, Transform child)
        {
            return true;
        }


        public static RectMask2D GetRectMaskForClippable(IClippable clippable)
        {
            return null;
        }


        public static void GetRectMasksForClip(RectMask2D clipper, List<RectMask2D> masks)
        {
        }
    }
}