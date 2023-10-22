using UnityEditor;
using UnityEngine;

public static class StatefulStyles
{
    static StatefulStyles()
    {
        //Centered bold label
        CenterLabel = new GUIStyle(EditorStyles.boldLabel);
        CenterLabel.alignment = TextAnchor.MiddleCenter;


        //Small label in corner, used for state group header
        SmallCornerLabel = new GUIStyle(EditorStyles.boldLabel);
        SmallCornerLabel.fontSize = 9;


        //smaller corner button with icon
        CornerButton = new GUIStyle(GUI.skin.button);
        CornerButton.margin.top += 1;
        CornerButton.padding = new RectOffset(1, 1, 1, 1);


        //Background used for states group
        Deep = new GUIStyle(GUI.skin.textField);
        Deep.onHover.background = Deep.normal.background;
        Deep.onHover.scaledBackgrounds = Deep.normal.scaledBackgrounds;
        Deep.hover.background = Deep.normal.background;
        Deep.hover.scaledBackgrounds = Deep.normal.scaledBackgrounds;

        //used for buttons with toggle behaviour(on/off)
        ToggleButton = new GUIStyle(GUI.skin.button);


        //Style used in the state background
        StateStyle = new GUIStyle(GUI.skin.button);


        //Style for the divider bar
        Divider = new GUIStyle(GUI.skin.textField);


        //Led light used in headers
        Led = new GUIStyle(GUIStyle.none);
    }

    public static GUIStyle Deep;

    public static GUIStyle StateStyle;

    public static GUIStyle ToggleButton;

    public static GUIStyle CenterLabel;

    public static GUIStyle SmallCornerLabel;

    public static GUIStyle CornerButton;

    public static GUIStyle Divider;

    public static GUIStyle Led;
}
