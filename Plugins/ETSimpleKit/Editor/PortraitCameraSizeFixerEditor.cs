using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(PortraitCameraSizeFixer))]
public class PortraitCameraSizeFixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PortraitCameraSizeFixer fixer = (PortraitCameraSizeFixer)target;

        if (GUILayout.Button("Create and bind target"))
        {
            if (fixer.targetObject != null)
            {
                EditorUtility.DisplayDialog("Create and bind target", "Target already assigned.", "OK");
            }
            else
            {
                // Create or find CameraTarget under World
                GameObject targetGo = ETEngine.Editor.WorldEditorUtility.CreateWorldObject("CameraTarget");
                if (targetGo == null)
                    targetGo = new GameObject("CameraTarget");

                // Ensure SpriteRenderer exists and make it invisible (alpha = 0)
                SpriteRenderer sr = targetGo.GetComponent<SpriteRenderer>();
                if (sr == null) sr = targetGo.AddComponent<SpriteRenderer>();
                // Try to assign Unity's default UI square sprite
                try
                {
                    Sprite defaultSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
                    if (defaultSprite != null)
                    {
                        sr.sprite = defaultSprite;
                    }
                }
                catch
                {
                    // ignore if builtin resource not found on this Unity version
                }
                sr.color = new Color(1f, 1f, 1f, 0f);
                // Place as first child under World (sibling index 0)
                if (targetGo.transform.parent != null)
                {
                    targetGo.transform.SetSiblingIndex(0);
                }
                // Put on Ignore Raycast layer if available
                int ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");
                if (ignoreLayer != -1) targetGo.layer = ignoreLayer;

                // Bind to the fixer
                fixer.targetObject = targetGo.transform;
                EditorUtility.SetDirty(fixer);
                EditorSceneManager.MarkSceneDirty(fixer.gameObject.scene);

                // Select the created target in the editor
                Selection.activeGameObject = targetGo;
            }
        }
    }
}
