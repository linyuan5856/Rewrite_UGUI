using System;
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
        [NonSerialized] private HashSet<IClippable> m_ClipTargets = new HashSet<IClippable>();

        [NonSerialized] private List<NewRectMask2D> m_Clippers = new List<NewRectMask2D>();

        [NonSerialized] private readonly NewRectangularVertexClipper clipperUtil = new NewRectangularVertexClipper();


        [NonSerialized] private bool m_ShouldRecalculateClipRects;

        [NonSerialized] private Rect m_LastClipRectCanvasSpace;

        [NonSerialized] private bool m_LastValidClipRect;

        [NonSerialized] private bool m_ForceClip;


        [NonSerialized] private RectTransform m_RectTransfrom;

        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransfrom == null)
                    m_RectTransfrom = this.GetComponent<RectTransform>();

                return m_RectTransfrom;
            }
        }

        public Rect canvasRect
        {
            get
            {
                Canvas canvas = null;
                var list = new List<Canvas>();
                gameObject.GetComponentsInParent(false, list);
                if (list.Count > 0)
                    canvas = list[list.Count - 1];


                return clipperUtil.GetCanvasRect(rectTransform, canvas);
            }
        }


        protected NewRectMask2D()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_ShouldRecalculateClipRects = true;
            ClipperRegistry.Register(this);
            NewMaskUtil.Notify2DMaskStateChanged(this);
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            m_Clippers.Clear();
            m_ClipTargets.Clear();
            ClipperRegistry.Unregister(this);
            NewMaskUtil.Notify2DMaskStateChanged(this);
        }

        public void PerformClipping()
        {
            if (m_ShouldRecalculateClipRects)
            {
                NewMaskUtil.GetRectMasksForClip(this, m_Clippers);
                m_ShouldRecalculateClipRects = false;
            }

            bool validRect = true;
            Rect clipRect = NewClipping.FindCullAndClipWorldRect(m_Clippers, out validRect);
            bool clipRectChanged = clipRect != m_LastClipRectCanvasSpace;
            if (clipRectChanged || m_ForceClip)
            {
                foreach (IClippable clipTarget in m_ClipTargets)
                    clipTarget.SetClipRect(clipRect, validRect);

                m_LastClipRectCanvasSpace = clipRect;
                m_LastValidClipRect = validRect;
            }

            foreach (IClippable clipTarget in m_ClipTargets)
            {
                var maskable = clipTarget as MaskableGraphic;
                if (maskable != null && !maskable.canvasRenderer.hasMoved && !clipRectChanged)
                    continue;

                clipTarget.Cull(m_LastClipRectCanvasSpace, m_LastValidClipRect);
            }
        }

        public void AddClippable(IClippable clippable)
        {
            if (clippable == null)
                return;
            m_ShouldRecalculateClipRects = true;

            if (!m_ClipTargets.Contains(clippable))
                m_ClipTargets.Add(clippable);

            m_ForceClip = true;
        }

        public void RemoveClippable(IClippable clippable)
        {
            if (clippable == null)
                return;
            m_ShouldRecalculateClipRects = true;

            clippable.SetClipRect(new Rect(), false);
            m_ClipTargets.Remove(clippable);

            m_ForceClip = false;
        }


        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            this.m_ShouldRecalculateClipRects = true;
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            this.m_ShouldRecalculateClipRects = true;
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_ShouldRecalculateClipRects = true;

            if (!IsActive())
                return;

            MaskUtilities.Notify2DMaskStateChanged(this);
        }

#endif
    }
}