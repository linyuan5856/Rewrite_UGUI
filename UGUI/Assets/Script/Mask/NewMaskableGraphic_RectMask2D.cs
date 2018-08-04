using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ReWriteUGUI
{
    namespace ReWriteUGUI
    {
        public class NewMaskableGraphic_RectMask2D : NewGraphic, IClippable
        {
            protected NewMaskableGraphic_RectMask2D(){ }

            [NonSerialized] private NewRectMask2D m_ParentMask;
            [NonSerialized] private bool m_Maskable = true;

            private RectTransform m_RectTransfrom;

            public RectTransform rectTransform
            {
                get
                {
                    if (m_RectTransfrom == null)
                        m_RectTransfrom = this.GetComponent<RectTransform>();

                    return m_RectTransfrom;
                }

                private set { m_RectTransfrom = value; }
            }

            readonly NewRectangularVertexClipper clipperutil = new NewRectangularVertexClipper();

            private Rect rootCanvasRect
            {
                get { return clipperutil.GetCanvasRect(rectTransform, canvas); }
            }

            public bool maskable
            {
                get { return m_Maskable; }
                set
                {
                    if (m_Maskable == value)
                        return;
                    m_Maskable = value;
                }
            }

            protected override void OnEnable()
            {
                base.OnEnable();
                UpdateClipParent();
            }

            protected override void OnDisable()
            {
                base.OnDisable();
                UpdateClipParent();
            }

            public void Cull(Rect clipRect, bool validRect)
            {
                var cull = !validRect || !clipRect.Overlaps(rootCanvasRect, true);
                this.UpdateCull(cull);
            }

            private void UpdateCull(bool cull)
            {
                var cullChanged = canvasRenderer.cull != cull;
                canvasRenderer.cull = cull;
                if (cullChanged)
                    SetVerticesDirty();
            }

            public void SetClipRect(Rect value, bool validRect)
            {
                if (validRect)
                    canvasRenderer.EnableRectClipping(value);
                else
                    canvasRenderer.DisableRectClipping();
            }


            public void RecalculateClipping()
            {
                UpdateClipParent();
            }


            private void UpdateClipParent()
            {
                NewRectMask2D newParent = ((maskable) && IsActive()) ? NewMaskUtil.GetRectMaskForClippable(this) : null;

                if (m_ParentMask != null && (newParent != m_ParentMask || !newParent.IsActive()))
                {
                    m_ParentMask.RemoveClippable(this);
                    UpdateCull(false);
                }

                if (newParent != null && newParent.IsActive())
                    newParent.AddClippable(this);

                m_ParentMask = newParent;
            }

#if UNITY_EDITOR
            protected override void OnValidate()
            {
                base.OnValidate();

                UpdateClipParent();
                SetMaterialDirty();
            }
#endif

            protected override void OnCanvasHierarchyChanged()
            {
                base.OnCanvasHierarchyChanged();

                if (!isActiveAndEnabled)
                    return;

                UpdateClipParent();
            }

            protected override void OnTransformParentChanged()
            {
                base.OnTransformParentChanged();

                if (!isActiveAndEnabled)
                    return;


                UpdateClipParent();
            }
        }
    }
}