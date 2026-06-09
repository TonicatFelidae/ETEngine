#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace ETEngine.Editor
{
    public class ProjectBuilder : EditorWindow
    {
        [MenuItem("ETools/Project Builder")]
        public static void ShowWindow()
        {
            GetWindow<ProjectBuilder>("Project Builder");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("? Info")) ProjectBuilderInfoWindow.Show();
            EditorGUILayout.Space();

            // --- ClassBuilder ---
            DrawSectionHeader("ClassBuilder");
            if (GUILayout.Button("Create General Object")) CreateGeneralObject();
            if (GUILayout.Button("Create Page")) CreatePage();
            if (GUILayout.Button("Create Popup")) CreatePopup();
            if (GUILayout.Button("Create Sheet")) CreateSheet();
            if (GUILayout.Button("Create Installer")) CreateInstaller();
            if (GUILayout.Button("Create Object Animator Controller")) CreateObjectAnimatorController();

            EditorGUILayout.Space();

            // --- Data ---
            DrawSectionHeader("Data");
            if (GUILayout.Button("Create GameDat")) CreateGameDat();
            if (GUILayout.Button("Create GameData")) CreateGameData();

            EditorGUILayout.Space();

            // --- SceneBuilder ---
            DrawSectionHeader("SceneBuilder");
            if (GUILayout.Button("Add Scene Installer")) SceneBuilderActions.AddSceneInstaller();
            if (GUILayout.Button("Add General Object")) SceneBuilderActions.AddGeneralObject();
        }

        private static void DrawSectionHeader(string title)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField($"--- {title} ---", EditorStyles.boldLabel);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1f));
            EditorGUILayout.Space(4);
        }

        // ── ClassBuilder ────────────────────────────────────────────────────

        private static void CreateGeneralObject()
        {
            StartNameEdit<CreateGeneralObjectAction>("New.cs");
        }

        private static void CreatePage()
        {
            StartNameEdit<CreatePageAction>("New.cs");
        }

        private static void CreatePopup()
        {
            StartNameEdit<CreatePopupAction>("New.cs");
        }

        private static void CreateSheet()
        {
            StartNameEdit<CreateSheetAction>("New.cs");
        }

        private static void CreateInstaller()
        {
            StartNameEdit<CreateInstallerAction>("New.cs");
        }

        private static void CreateObjectAnimatorController()
        {
            StartNameEdit<CreateObjectAnimatorControllerAction>("NewObjectAnimatorController.cs");
        }

        // ── Data ─────────────────────────────────────────────────────────────

        private static void CreateGameDat()
        {
            CreateGameDatAction.Create();
        }

        private static void CreateGameData()
        {
            CreateGameDataAction.Create();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static void StartNameEdit<T>(string defaultFileName) where T : EndNameEditAction
        {
            string path = GetSelectedPathOrFallback();
            string filePath = $"{path}/{defaultFileName}".Replace("\\", "/");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<T>(),
                filePath,
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                null
            );
        }

        private static string GetSelectedPathOrFallback()
        {
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (Directory.Exists(path)) return path;
                if (File.Exists(path)) return Path.GetDirectoryName(path);
            }
            return "Assets";
        }
    }
}
#endif