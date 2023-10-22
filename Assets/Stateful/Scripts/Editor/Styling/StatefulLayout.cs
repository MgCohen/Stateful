using UnityEditor;
using UnityEngine;
using System.Linq;

public static class StatefulLayout
{
    public static bool IconButton(string iconName, float size, params GUILayoutOption[] options)
    {
        return IconButton(iconName, null, size, options);
    }

    public static bool IconButton(string iconName, string tooltip, float size, params GUILayoutOption[] options)
    {
        GUIContent icon = EditorGUIUtility.IconContent(iconName, tooltip);
        GUILayoutOption height = GUILayout.Height(size);
        GUILayoutOption width = GUILayout.Width(size);
        options = options.Append(height).Append(width).ToArray();
        return GUILayout.Button(icon, StatefulStyles.CornerButton, options);
    }


    public static bool ToggleButton(string label, bool state)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Toggle(state, StatefulStyles.ToggleButton);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        EditorGUI.LabelField(lastRect, label, StatefulStyles.CenterLabel);
        return EditorGUI.EndChangeCheck();
    }

    public static void Divider()
    {
        EditorGUI.BeginDisabledGroup(true);
        GUILayout.Box(GUIContent.none, StatefulStyles.Divider, GUILayout.Height(2));
        EditorGUI.EndDisabledGroup();
    }

    public static void AllignedLabel(string label, TextAnchor anchor, GUIStyle style = null, params GUILayoutOption[] options)
    {
        style ??= GUI.skin.label;
        style = new GUIStyle(style);
        style.alignment = anchor;
        EditorGUILayout.LabelField(label, style, options);
    }

    public static void Led(LedStyle led)
    {
        string iconName = StatefulLayoutUtility.GetLedIconName(led);
        GUIContent ledIcon = EditorGUIUtility.IconContent(iconName);
        GUIContent ledBackground = EditorGUIUtility.IconContent("d_lightRim");
        float size = EditorGUIUtility.singleLineHeight;
        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(size));
        GUI.Box(rect, ledIcon, StatefulStyles.Led);
        GUI.Box(rect, ledBackground, StatefulStyles.Led);
    }

    public static bool LedButton(LedStyle led)
    {
        Led(led);
        Rect lastRect = GUILayoutUtility.GetLastRect();
        lastRect.width = EditorGUIUtility.singleLineHeight;
        return GUI.Button(lastRect, GUIContent.none, StatefulStyles.Led);
    }

}
