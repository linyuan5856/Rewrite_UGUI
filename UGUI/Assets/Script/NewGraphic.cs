using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using System.Reflection;

#endif

namespace ReWriteUGUI
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(CanvasRenderer))]
    public class NewGraphic : UIBehaviour, ICanvasElement
    {
        private static Material s_Defult_UI_Material;
        private static Texture2D s_Texture_White;
        private static Mesh s_Work_Mesh;
        private static VertexHelper vh = new VertexHelper();

        [SerializeField] private Material m_Material;
        [SerializeField] private Texture2D m_MainTexture;

        private RectTransform m_RectTransform;
        private CanvasRenderer m_CanvasRenderer;
        [SerializeField] private Color mColor = Color.white;
        private Canvas mCanvas;

        [NonSerialized] private bool isVerticesDirty;
        [NonSerialized] private bool isMaterialDirty;

        private Mesh WorkMesh
        {
            get
            {
                if (s_Work_Mesh == null)
                {
                    s_Work_Mesh = new Mesh();
                    s_Work_Mesh.name = "Shared UI Mesh";
                    s_Work_Mesh.hideFlags = HideFlags.HideAndDontSave;
                }

                return s_Work_Mesh;
            }
        }

        protected virtual Texture2D MainTexture
        {
            get
            {
                if (m_MainTexture != null)
                    return m_MainTexture;
                if (s_Texture_White == null)
                    s_Texture_White = Texture2D.whiteTexture;
                return s_Texture_White;
            }

            set
            {
                if (m_MainTexture == value)
                    return;
                m_MainTexture = value;
                SetMaterialDirty();
            }
        }

        private Material DefaultMaterial
        {
            get
            {
                if (s_Defult_UI_Material == null)
                    s_Defult_UI_Material = Canvas.GetDefaultCanvasMaterial();
                return s_Defult_UI_Material;
            }
        }

        protected virtual Material material
        {
            get { return m_Material != null ? m_Material : DefaultMaterial; }

            set
            {
                if (m_Material == value)
                    return;

                m_Material = value;
                SetMaterialDirty();
            }
        }

        public virtual Material materialForRendering
        {
            get
            {
                var components = new List<Component>();
                GetComponents(typeof(IMaterialModifier), components);

                var currentMat = material;
                for (var i = 0; i < components.Count; i++)
                    currentMat = (components[i] as IMaterialModifier).GetModifiedMaterial(currentMat);
                return currentMat;
            }
        }

        public CanvasRenderer canvasRenderer
        {
            get
            {
                if (m_CanvasRenderer == null)
                    m_CanvasRenderer = this.GetComponent<CanvasRenderer>();

                return m_CanvasRenderer;
            }
        }

        protected RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null)
                    m_RectTransform = this.GetComponent<RectTransform>();
                return m_RectTransform;
            }
        }

        protected Canvas canvas
        {
            get
            {
                if (mCanvas == null)
                    CacheCanvas();
                return mCanvas;
            }
        }


        void CacheCanvas()
        {
            var list = NewListPool<Canvas>.Get();
            this.GetComponentsInParent(false, list);

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null && list[i].isActiveAndEnabled)
                {
                    mCanvas = list[i];
                    break;
                }
            }

            NewListPool<Canvas>.Release(list);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CacheCanvas();
            SetAllDirty();
        }

        protected override void OnDisable()
        {
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
            if (canvasRenderer)
                canvasRenderer.Clear();
            base.OnDisable();
        }


        protected void SetAllDirty()
        {
            //Debug.LogWarning("Set All Dirty");
            SetLayoutDirty();
            SetVerticesDirty();
            SetMaterialDirty();
        }

        protected void SetLayoutDirty()
        {
            if (!IsActive())
                return;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        protected void SetVerticesDirty()
        {
            if (!IsActive())
                return;
            isVerticesDirty = true;
            CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
        }

        public void SetMaterialDirty()
        {
            if (!IsActive())
                return;
            isMaterialDirty = true;
            CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
        }

        private void UpdateMaterial()
        {
            if (!IsActive())
                return;

            canvasRenderer.materialCount = 1;
            canvasRenderer.SetMaterial(materialForRendering, 0);
            canvasRenderer.SetTexture(MainTexture);
        }

        private void UpdateGeometry()
        {
            vh.Clear();
            OnPopulateMesh(vh);
            vh.FillMesh(WorkMesh);
            canvasRenderer.SetMesh(WorkMesh);
        }

        protected virtual void OnPopulateMesh(VertexHelper vh)
        {
            var r = rectTransform.rect;
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
            Color32 vertColor = mColor;
            vh.AddVert(new Vector3(v.x, v.y), vertColor, new Vector2(0, 0));
            vh.AddVert(new Vector3(v.x, v.w), vertColor, new Vector2(0, 1));
            vh.AddVert(new Vector3(v.z, v.w), vertColor, new Vector2(1, 1));
            vh.AddVert(new Vector3(v.z, v.y), vertColor, new Vector2(1, 0));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

        public virtual void Rebuild(CanvasUpdate update)
        {
            if (canvasRenderer.cull)
                return;
            //Debug.LogWarning("Rebuild");
            switch (update)
            {
                case CanvasUpdate.PreRender:
                    if (isVerticesDirty)
                    {
                        this.UpdateGeometry();
                        isVerticesDirty = false;
                    }

                    if (isMaterialDirty)
                    {
                        this.UpdateMaterial();
                        isMaterialDirty = false;
                    }

                    break;
            }
        }

        public virtual void LayoutComplete()
        {
        }

        public virtual void GraphicUpdateComplete()
        {
        }


        protected override void OnCanvasHierarchyChanged()
        {
            Debug.Log("OnCanvasHierarchyChanged");

            mCanvas = null;

            if (!IsActive())
                return;

            CacheCanvas();
        }


        protected override void OnBeforeTransformParentChanged()
        {
            Debug.Log("OnBeforeTransformParentChanged");

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

        protected override void OnTransformParentChanged()
        {
            Debug.Log("OnTransformParentChanged");

            base.OnTransformParentChanged();

            mCanvas = null;

            if (!IsActive())
                return;

            CacheCanvas();
            SetAllDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            Debug.Log("OnRectTransformDimensionsChange");
            if (gameObject.activeInHierarchy)
            {
                if (CanvasUpdateRegistry.IsRebuildingLayout())
                    SetVerticesDirty();
                else
                {
                    SetVerticesDirty();
                    SetLayoutDirty();
                }
            }
        }

        protected override void OnDidApplyAnimationProperties()
        {
            Debug.Log("OnDidApplyAnimationProperties");

            SetAllDirty();
        }

#if UNITY_EDITOR
        public virtual void OnRequestRebuild()
        {
            Debug.Log("OnRequestRebuild");

            var mbs = this.GetComponents<MonoBehaviour>();
            foreach (var mb in mbs)
            {
                if (mb == null)
                    continue;
                MethodInfo methodInfo = mb.GetType().GetMethod("OnValidate",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                if (methodInfo != null)
                    methodInfo.Invoke(mb, null);
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            this.SetAllDirty();
        }

        protected override void Reset()
        {
            base.Reset();
            this.SetAllDirty();
        }
#endif
    }
}