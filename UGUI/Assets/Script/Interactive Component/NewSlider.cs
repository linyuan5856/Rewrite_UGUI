using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewSlider : UIBehaviour
{
    [SerializeField] private RectTransform m_FillRect;
    [SerializeField] private RectTransform m_HandleRect;
    [SerializeField] private float m_Value;
    [SerializeField] private float m_MinValue;
    [SerializeField] private float m_MaxValue;


    private Image m_FillImage;
 

    protected override void OnEnable()
    {
        base.OnEnable();

        this.UpdateCachedReferences();
        this.Set(m_Value);
        UpdateVisual();
    }


    void UpdateCachedReferences()
    {
        m_FillImage = null;
        if (m_FillRect)         
            m_FillImage = m_FillRect.GetComponent<Image>();
    }

    private float Clamp(float input)
    {
        return Mathf.Clamp(input, m_MinValue, m_MaxValue);
    }

    protected virtual void Set(float value)
    {
        float newValue = Clamp(value);
        m_Value = newValue;
        this.UpdateVisual();
    }


    private void UpdateVisual()
    {
        Vector2 anchorMin = Vector2.zero;
        Vector2 anchorMax = Vector2.one;

        if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
            m_FillImage.fillAmount = m_Value;
        else
            anchorMax[0] = m_Value;


        m_FillRect.anchorMin = anchorMin;
        m_FillRect.anchorMax = anchorMax;


        anchorMin[0] = anchorMax[0] = m_Value;
        m_HandleRect.anchorMin = anchorMin;
        m_HandleRect.anchorMax = anchorMax;
    }


#if UNITY_EDITOR

    protected override void OnValidate()
    {
        base.OnValidate();
        if (!IsActive())
            return;
        UpdateCachedReferences();
        this.Set(m_Value);
        UpdateVisual();
    }

#endif
}