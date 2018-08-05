using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReWriteUGUI
{
    public static class NewStencilMaterial
    {
        private class MatEntry
        {
            public Material baseMat;
            public Material customMat;
            public int count;

            public int stencilId;
            public StencilOp operation = StencilOp.Keep;
            public CompareFunction compareFunction = CompareFunction.Always;
            public int readMask;
            public int writeMask;
            public bool useAlphaClip;
            public ColorWriteMask colorMask;
        }

        private static bool CheckMaterial(Material baseMat)
        {
            bool canPass = true;
            if (!baseMat.HasProperty("_Stencil"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _Stencil property", baseMat);
                canPass = false;
            }

            if (!baseMat.HasProperty("_StencilOp"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _StencilOp property", baseMat);
                canPass = false;
            }

            if (!baseMat.HasProperty("_StencilComp"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _StencilComp property", baseMat);
                canPass = false;
            }

            if (!baseMat.HasProperty("_StencilReadMask"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _StencilReadMask property", baseMat);
                canPass = false;
            }

            if (!baseMat.HasProperty("_StencilWriteMask"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _StencilWriteMask property", baseMat);
                canPass = false;
            }

            if (!baseMat.HasProperty("_ColorMask"))
            {
                Debug.LogWarning("Material " + baseMat.name + " doesn't have _ColorMask property", baseMat);
                canPass = false;
            }

            return canPass;
        }

        private static List<MatEntry> m_List = new List<MatEntry>();


        public static Material Add(Material baseMat, int stencilID, StencilOp operation,
            CompareFunction compareFunction, ColorWriteMask colorWriteMask)
        {
            return Add(baseMat, stencilID, operation, compareFunction, colorWriteMask, 255, 255);
        }


        public static Material Add(Material baseMat, int stencilID, StencilOp operation,
            CompareFunction compareFunction, ColorWriteMask colorWriteMask, int readMask, int writeMask)
        {
            if ((stencilID <= 0 && colorWriteMask == ColorWriteMask.All) || baseMat == null)
                return baseMat;

            if (!CheckMaterial(baseMat))
                return baseMat;


            for (int i = 0; i < m_List.Count; ++i)
            {
                MatEntry ent = m_List[i];

                if (ent.baseMat == baseMat
                    && ent.stencilId == stencilID
                    && ent.operation == operation
                    && ent.compareFunction == compareFunction
                    && ent.readMask == readMask
                    && ent.writeMask == writeMask
                    && ent.colorMask == colorWriteMask)
                {
                    ++ent.count;
                    return ent.customMat;
                }
            }

            var newEnt = new MatEntry();
            newEnt.count = 1;
            newEnt.baseMat = baseMat;
            newEnt.customMat = new Material(baseMat);
            newEnt.customMat.hideFlags = HideFlags.HideAndDontSave;
            newEnt.stencilId = stencilID;
            newEnt.operation = operation;
            newEnt.compareFunction = compareFunction;
            newEnt.readMask = readMask;
            newEnt.writeMask = writeMask;
            newEnt.colorMask = colorWriteMask;
            newEnt.useAlphaClip = operation != StencilOp.Keep && writeMask > 0;

            newEnt.customMat.name = string.Format(
                "Stencil Id:{0}, Op:{1}, Comp:{2}, WriteMask:{3}, ReadMask:{4}, ColorMask:{5} AlphaClip:{6} ({7})",
                stencilID, operation, compareFunction, writeMask, readMask, colorWriteMask, newEnt.useAlphaClip,
                baseMat.name);

            newEnt.customMat.SetInt("_Stencil", stencilID);
            newEnt.customMat.SetInt("_StencilOp", (int) operation);
            newEnt.customMat.SetInt("_StencilComp", (int) compareFunction);
            newEnt.customMat.SetInt("_StencilReadMask", readMask);
            newEnt.customMat.SetInt("_StencilWriteMask", writeMask);
            newEnt.customMat.SetInt("_ColorMask", (int) colorWriteMask);

         
            if (newEnt.useAlphaClip)
                newEnt.customMat.EnableKeyword("UNITY_UI_ALPHACLIP");
            else
                newEnt.customMat.DisableKeyword("UNITY_UI_ALPHACLIP");

            m_List.Add(newEnt);
            return newEnt.customMat;
        }


        public static void Remove(Material customMat)
        {
            if (customMat == null)
                return;

            for (int i = 0; i < m_List.Count; ++i)
            {
                MatEntry ent = m_List[i];

                if (ent.customMat != customMat)
                    continue;

                if (--ent.count == 0)
                {
                    DestroyImmediate(ent.customMat);
                    ent.baseMat = null;
                    m_List.RemoveAt(i);
                }

                return;
            }
        }

        public static void ClearAll()
        {
            for (int i = 0; i < m_List.Count; ++i)
            {
                MatEntry ent = m_List[i];

                DestroyImmediate(ent.customMat);
                ent.baseMat = null;
            }

            m_List.Clear();
        }

        static public void DestroyImmediate(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                if (Application.isEditor) UnityEngine.Object.DestroyImmediate(obj);
                else UnityEngine.Object.Destroy(obj);
            }
        }
    }
}