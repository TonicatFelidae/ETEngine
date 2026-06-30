using ETEngine;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TutorialStep))]
public class TutorialStepDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        SerializedProperty targetProp = property.FindPropertyRelative("target");
        SerializedProperty showHighlightProp = property.FindPropertyRelative("showHighlight");
        SerializedProperty showStandoutProp = property.FindPropertyRelative("showStandout");
        SerializedProperty showTextProp = property.FindPropertyRelative("showText");
        SerializedProperty instructionTextProp = property.FindPropertyRelative("instructionText");
        SerializedProperty showPopupProp = property.FindPropertyRelative("showPopup");
        SerializedProperty popupPrefabProp = property.FindPropertyRelative("pp_popup");
        SerializedProperty popupOffsetProp = property.FindPropertyRelative("popupOffset");

        DrawField(ref rect, targetProp, spacing);

        DrawField(ref rect, showHighlightProp, spacing);
        DrawField(ref rect, showStandoutProp, spacing);
        DrawField(ref rect, showTextProp, spacing);

        if (showTextProp != null && showTextProp.boolValue)
        {
            DrawField(ref rect, instructionTextProp, spacing);
        }

        DrawField(ref rect, showPopupProp, spacing);

        if (showPopupProp != null && showPopupProp.boolValue)
        {
            DrawField(ref rect, popupPrefabProp, spacing);
            DrawField(ref rect, popupOffsetProp, spacing);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float line = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        int lines = 0;

        lines++; // target

        lines++; // showHighlight
        lines++; // showStandout
        lines++; // showText

        SerializedProperty showTextProp = property.FindPropertyRelative("showText");
        if (showTextProp != null && showTextProp.boolValue)
        {
            lines++; // instructionText
        }

        lines++; // showPopup

        SerializedProperty showPopupProp = property.FindPropertyRelative("showPopup");
        if (showPopupProp != null && showPopupProp.boolValue)
        {
            lines += 2; // pp_popup + popupOffset
        }

        return (lines * line) + ((lines - 1) * spacing);
    }

    private static void DrawField(ref Rect rect, SerializedProperty prop, float spacing)
    {
        if (prop == null)
        {
            return;
        }

        EditorGUI.PropertyField(rect, prop, true);
        rect.y += EditorGUI.GetPropertyHeight(prop, true) + spacing;
    }
}