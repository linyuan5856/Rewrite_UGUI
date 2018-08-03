using ReWriteUGUI;
using UnityEditor;

[CustomEditor(typeof(NewToggle), true)]
[CanEditMultipleObjects]
public class ToggleEditor : Editor
{
    SerializedProperty m_OnValueChangedProperty;
    SerializedProperty m_GraphicProperty;
    SerializedProperty m_IsOnProperty;

    SerializedProperty m_GroupProperty;

    protected virtual void OnEnable()
    {
        this.m_OnValueChangedProperty = serializedObject.FindProperty("onValueChanged");
        this.m_GraphicProperty = serializedObject.FindProperty("graphic");
        this.m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
        this.m_GroupProperty=serializedObject.FindProperty("m_Group");
    }

    public override void OnInspectorGUI()
    {
    
        EditorGUILayout.Space();

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_IsOnProperty);
        EditorGUILayout.PropertyField(m_GraphicProperty);
        EditorGUILayout.PropertyField(m_GroupProperty);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_OnValueChangedProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
