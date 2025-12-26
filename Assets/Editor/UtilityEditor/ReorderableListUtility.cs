using UnityEditor;
using UnityEngine;

namespace VurbiriEditor
{
	public static class ReorderableListUtility
	{
        private static readonly Color s_oddFocusedColor = new Color32(52, 52, 82, 255), s_evenFocusedColor = new Color32(60, 60, 90, 255);
        private static readonly Color s_oddActiveColor  = new Color32(62, 62, 68, 255), s_evenActiveColor  = new Color32(70, 70, 76, 255);
        private static readonly Color s_oddDefaultColor = new Color32(62, 62, 62, 255), s_evenDefaultColor = new Color32(70, 70, 70, 255);

        public static void DrawBackground(Rect size, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
                EditorGUI.DrawRect(size, (index & 1) == 1 ? s_oddFocusedColor : s_evenFocusedColor);
            else if (isActive)
                EditorGUI.DrawRect(size, (index & 1) == 1 ? s_oddActiveColor  : s_evenActiveColor );
            else
                EditorGUI.DrawRect(size, (index & 1) == 1 ? s_oddDefaultColor : s_evenDefaultColor);
        }
    }
}
