#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ETEngine.Editor
{
    internal static class SceneBuilderActions
    {
        public static void AddSceneInstaller()
        {
            var go = new GameObject("SceneInstaller");
            Undo.RegisterCreatedObjectUndo(go, "Add SceneInstaller");
            Selection.activeGameObject = go;
        }

        public static void AddGeneralObject()
        {
            var go = new GameObject("GeneralObject");
            Undo.RegisterCreatedObjectUndo(go, "Add GeneralObject");
            Selection.activeGameObject = go;
        }

        public static void AddDevSupport()
        {
            var go = new GameObject("DevSupport");
            go.AddComponent<ET.DevSupport>();
            Undo.RegisterCreatedObjectUndo(go, "Add DevSupport");
            Selection.activeGameObject = go;
        }
    }
}
#endif
