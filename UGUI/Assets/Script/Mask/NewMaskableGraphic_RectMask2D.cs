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
            [NonSerialized] private RectMask2D m_ParentMask;
            [NonSerialized] private bool m_Maskable = true;

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

            readonly Vector3[] m_Corners = new Vector3[4];

            private Rect rootCanvasRect
            {
                get
                {
                    rectTransform.GetWorldCorners(m_Corners);

                    if (canvas)
                    {
                        Canvas rootCanvas = canvas.rootCanvas;
                        for (int i = 0; i < 4; ++i)
                            m_Corners[i] = rootCanvas.transform.InverseTransformPoint(m_Corners[i]);
                    }

                    return new Rect(m_Corners[0].x, m_Corners[0].y, m_Corners[2].x - m_Corners[0].x,
                        m_Corners[2].y - m_Corners[0].y);
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


            public RectTransform rectTransform { get; private set; }


            private void UpdateClipParent()
            {
                var newParent = ((maskable) && IsActive()) ? NewMaskUtil.GetRectMaskForClippable(this) : null;

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