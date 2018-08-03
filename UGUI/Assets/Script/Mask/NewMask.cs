using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReWriteUGUI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class NewMask : UIBehaviour, IMaterialModifier
    {
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            return null;
        }
    }
}