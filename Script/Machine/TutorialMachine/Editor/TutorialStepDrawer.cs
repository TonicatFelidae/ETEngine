using ETEngine;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
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
        SerializedProperty showSpotLightProp = property.FindPropertyRelative("showSpotLight");
        SerializedProperty showTextProp = property.FindPropertyRelative("showText");
        SerializedProperty instructionTextProp = property.FindPropertyRelative("instructionText");
        SerializedProperty showPopupProp = property.FindPropertyRelative("showPopup");
        SerializedProperty popupPrefabProp = property.FindPropertyRelative("pp_popup");
        SerializedProperty showOverlayProp = property.FindPropertyRelative("showOverlay");
        SerializedProperty overlayProp = property.FindPropertyRelative("overlay");
        SerializedProperty popupOffsetProp = property.FindPropertyRelative("popupOffset");
        SerializedProperty nextStepTriggerTypeProp = property.FindPropertyRelative("nextStepTriggerType");
        SerializedProperty onCompletedProp = property.FindPropertyRelative("onCompleted");
        SerializedProperty onCompletedFeedbackProp = property.FindPropertyRelative("onCompletedFeedback");

        DrawField(ref rect, targetProp, spacing);

        DrawField(ref rect, showHighlightProp, spacing);
        DrawField(ref rect, showStandoutProp, spacing);
        DrawField(ref rect, showSpotLightProp, spacing);
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

        DrawField(ref rect, showOverlayProp, spacing);

        if (showOverlayProp != null && showOverlayProp.boolValue)
        {
            DrawField(ref rect, overlayProp, spacing);
        }

        DrawField(ref rect, nextStepTriggerTypeProp, spacing);
        DrawField(ref rect, onCompletedProp, spacing);

        if (onCompletedProp != null && (OnTutorialStepComplete)onCompletedProp.enumValueIndex == OnTutorialStepComplete.Feedback)
        {
            DrawField(ref rect, onCompletedFeedbackProp, spacing);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float height = 0f;

        height += GetFieldHeight(property.FindPropertyRelative("target"), spacing);
        height += GetFieldHeight(property.FindPropertyRelative("showHighlight"), spacing);
        height += GetFieldHeight(property.FindPropertyRelative("showStandout"), spacing);
        height += GetFieldHeight(property.FindPropertyRelative("showSpotLight"), spacing);
        height += GetFieldHeight(property.FindPropertyRelative("showText"), spacing);

        SerializedProperty showTextProp = property.FindPropertyRelative("showText");
        if (showTextProp != null && showTextProp.boolValue)
        {
            height += GetFieldHeight(property.FindPropertyRelative("instructionText"), spacing);
        }

        height += GetFieldHeight(property.FindPropertyRelative("showPopup"), spacing);

        SerializedProperty showPopupProp = property.FindPropertyRelative("showPopup");
        if (showPopupProp != null && showPopupProp.boolValue)
        {
            height += GetFieldHeight(property.FindPropertyRelative("pp_popup"), spacing);
            height += GetFieldHeight(property.FindPropertyRelative("popupOffset"), spacing);
        }

        height += GetFieldHeight(property.FindPropertyRelative("showOverlay"), spacing);

        SerializedProperty showOverlayProp = property.FindPropertyRelative("showOverlay");
        if (showOverlayProp != null && showOverlayProp.boolValue)
        {
            height += GetFieldHeight(property.FindPropertyRelative("overlay"), spacing);
        }

        height += GetFieldHeight(property.FindPropertyRelative("nextStepTriggerType"), spacing);

        SerializedProperty onCompletedProp = property.FindPropertyRelative("onCompleted");
        height += GetFieldHeight(onCompletedProp, spacing);

        if (onCompletedProp != null && (OnTutorialStepComplete)onCompletedProp.enumValueIndex == OnTutorialStepComplete.Feedback)
        {
            height += GetFieldHeight(property.FindPropertyRelative("onCompletedFeedback"), spacing);
        }

        return Mathf.Max(0f, height - spacing);
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

    private static float GetFieldHeight(SerializedProperty prop, float spacing)
    {
        if (prop == null)
        {
            return 0f;
        }

        return EditorGUI.GetPropertyHeight(prop, true) + spacing;
    }
}
#endif