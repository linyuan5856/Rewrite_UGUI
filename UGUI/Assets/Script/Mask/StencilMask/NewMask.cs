using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ReWriteUGUI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class NewMask : UIBehaviour, IMaterialModifier
    {
        [NonSerialized] private RectTransform m_RectTransform;

        public RectTransform rectTransform
        {
            get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
        }

        [SerializeField] private bool m_ShowMaskGraphic = true;

        public bool showMaskGraphic
        {
            get { return m_ShowMaskGraphic; }
            set
            {
                if (m_ShowMaskGraphic == value)
                    return;

                m_ShowMaskGraphic = value;
                if (graphic != null)
                    graphic.SetMaterialDirty();
            }
        }

        [NonSerialized] private NewGraphic m_Graphic;

        public NewGraphic graphic
        {
            get { return m_Graphic ?? (m_Graphic = GetComponent<NewGraphic>()); }
        }

        [NonSerialized] private Material m_MaskMaterial;

        [NonSerialized] private Material m_UnmaskMaterial;

        protected NewMask()
        {
        }

        public virtual bool MaskEnabled()
        {
            return IsActive() && graphic != null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (graphic != null)
            {
                graphic.canvasRenderer.hasPopInstruction = true;
                graphic.SetMaterialDirty();
            }

            NewMaskUtil.NotifyStencilStateChanged(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (graphic != null)
            {
                graphic.SetMaterialDirty();
                graphic.canvasRenderer.hasPopInstruction = false;
                graphic.canvasRenderer.popMaterialCount = 0;
            }

            NewStencilMaterial.Remove(m_MaskMaterial);
            m_MaskMaterial = null;
            NewStencilMaterial.Remove(m_UnmaskMaterial);
            m_UnmaskMaterial = null;
            NewMaskUtil.NotifyStencilStateChanged(this);
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (!MaskEnabled())
                return baseMaterial;

            var rootSortCanvas = NewMaskUtil.FindRootSortOverrideCanvas(transform);
            var stencilDepth = NewMaskUtil.GetStencilDepth(transform, rootSortCanvas);
            if (stencilDepth >= 8)
            {
                Debug.LogError("Attempting to use a stencil mask with depth > 8", gameObject);
                return baseMaterial;
            }

            int desiredStencilBit = 1 << stencilDepth;


            if (desiredStencilBit == 1)
            {
                var maskMaterial = NewStencilMaterial.Add(baseMaterial, 1, StencilOp.Replace, CompareFunction.Always,
                    m_ShowMaskGraphic ? ColorWriteMask.All : 0);
                NewStencilMaterial.Remove(m_MaskMaterial);
                m_MaskMaterial = maskMaterial;

                var unmaskMaterial = NewStencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, 0);
                NewStencilMaterial.Remove(m_UnmaskMaterial);
                m_UnmaskMaterial = unmaskMaterial;
                graphic.canvasRenderer.popMaterialCount = 1;
                graphic.canvasRenderer.SetPopMaterial(m_UnmaskMaterial, 0);

                return m_MaskMaterial;
            }


            var maskMaterial2 = NewStencilMaterial.Add(baseMaterial, desiredStencilBit | (desiredStencilBit - 1),
                StencilOp.Replace, CompareFunction.Equal, m_ShowMaskGraphic ? ColorWriteMask.All : 0,
                desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
            NewStencilMaterial.Remove(m_MaskMaterial);
            m_MaskMaterial = maskMaterial2;

            graphic.canvasRenderer.hasPopInstruction = true;
            var unmaskMaterial2 = NewStencilMaterial.Add(baseMaterial, desiredStencilBit - 1, StencilOp.Replace,
                CompareFunction.Equal, 0, desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
            NewStencilMaterial.Remove(m_UnmaskMaterial);
            m_UnmaskMaterial = unmaskMaterial2;
            graphic.canvasRenderer.popMaterialCount = 1;
            graphic.canvasRenderer.SetPopMaterial(m_UnmaskMaterial, 0);

            return m_MaskMaterial;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!IsActive())
                return;

            if (graphic != null)
                graphic.SetMaterialDirty();

            MaskUtilities.NotifyStencilStateChanged(this);
        }

#endif
    }
}