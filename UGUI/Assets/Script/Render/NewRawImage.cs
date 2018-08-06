using UnityEngine;
using UnityEngine.UI;

namespace ReWriteUGUI
{
    public class NewRawImage : NewGraphic
    {
        [SerializeField] public Rect rect = new Rect(0, 0, 1, 1);


        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            UIVertex vert = new UIVertex();
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                Vector2 newUV = Vector2.zero;
                switch (i)
                {
                    case 0:
                        newUV = new Vector2(rect.xMin, rect.yMin);
                        break;
                    case 1:
                        newUV = new Vector2(rect.xMin, rect.yMax);
                        break;
                    case 2:
                        newUV = new Vector2(rect.xMax, rect.yMax);
                        break;
                    case 3:
                        newUV = new Vector2(rect.xMax, rect.yMin);
                        break;
                }

                vert.uv0 = newUV;
                vh.SetUIVertex(vert,i);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();          
            this.SetVerticesDirty();
        }
#endif
    }
}