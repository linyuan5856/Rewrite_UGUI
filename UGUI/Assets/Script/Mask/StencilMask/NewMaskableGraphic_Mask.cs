using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ReWriteUGUI
{
    public class NewMaskableGraphic_Mask : UIBehaviour, IMaterialModifier
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            return baseMaterial;
        }
    }
}