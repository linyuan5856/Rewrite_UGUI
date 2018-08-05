using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ReWriteUGUI
{
    public class NewMaskableGraphic_Mask : NewGraphic, IMaterialModifier, IMaskable
    {
        [NonSerialized] private bool m_ShouldRecalculateStencil = true;
        [NonSerialized] protected Material m_MaskMaterial;

        [NonSerialized] private bool m_makeable=true;
        [NonSerialized] private int m_StencilValue;

       
        public bool maskable
        {
            get { return m_makeable; }
            set
            {
                if (m_makeable == value)
                    return;
                maskable = value;
                m_ShouldRecalculateStencil = true;
                SetMaterialDirty();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_ShouldRecalculateStencil = true;
            SetMaterialDirty();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            m_ShouldRecalculateStencil = true;
            SetMaterialDirty();
            NewStencilMaterial.Remove(m_MaskMaterial);
            m_MaskMaterial = null;

        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            var targetMat = baseMaterial;
            if (m_ShouldRecalculateStencil)
            {
                var rootCanvas = NewMaskUtil.FindRootSortOverrideCanvas(transform);
                m_StencilValue = maskable ? NewMaskUtil.GetStencilDepth(transform, rootCanvas) : 0;
                m_ShouldRecalculateStencil = false;
            }

            var mask = this.GetComponent<NewMask>();
            if (m_StencilValue > 0 && (mask == null || !mask.IsActive()))
            {
                var maskMat = NewStencilMaterial.Add(baseMaterial,
                    (1 << m_StencilValue) - 1,
                    StencilOp.Keep,
                    CompareFunction.Equal, ColorWriteMask.All,
                    (1 << m_StencilValue) - 1,
                    0);
                NewStencilMaterial.Remove(m_MaskMaterial);
                targetMat = maskMat;
                m_MaskMaterial = maskMat;
            }


            return targetMat;
        }

        public void RecalculateMasking()
        {
            m_ShouldRecalculateStencil = true;
            SetMaterialDirty();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_ShouldRecalculateStencil = true;
            SetMaterialDirty();
        }
#endif
    }
}