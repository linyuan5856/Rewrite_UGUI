using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReWriteUGUI
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class NewRectMask2D : UIBehaviour, IClipper
    {
        
        public void PerformClipping()
        {
        }
    }
}