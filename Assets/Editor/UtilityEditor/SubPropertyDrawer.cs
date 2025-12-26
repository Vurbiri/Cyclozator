using UnityEditor;
using UnityEngine;

namespace VurbiriEditor
{
    public abstract class SubPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            property.isExpanded = true;

            int count = property.Copy().CountInProperty();
            EditorGUI.BeginProperty(position, label, property);
            while (--count > 0)
            {
                property.Next(true);
                EditorGUI.PropertyField(position, property);
                position.y += EditorGUI.GetPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property) - EditorGUIUtility.singleLineHeight;
        }
    }
}
