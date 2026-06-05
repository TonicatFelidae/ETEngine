#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ETEngine.Editor
{
    internal class ProjectBuilderInfoWindow : EditorWindow
    {
        private Vector2 _scroll;

        private const string InfoText =
@"Project Builder – Quick Reference
══════════════════════════════════

ClassBuilder
────────────
 • General Object
   → Assets/Game/___Script___/{Name}GeneralObject.cs
   Base: MonoBehaviour + ISceneEntryPoint (async init)

 • Page
   → Assets/Game/___Script___/UI/{Name}Page.cs
   Base: UnityScreenNavigator Page

 • Popup
   → Assets/Game/___Script___/UI/{Name}Popup.cs
   Base: UnityScreenNavigator Popup (Modal)

 • Sheet
   → Assets/Game/___Script___/UI/{Name}Sheet.cs
   Base: UnityScreenNavigator Sheet

 • Installer
   → Assets/Game/___Script___/Installer/{Name}Installer.cs
   Base: MonoBehaviour

 • Object Animator Controller
   → Assets/Game/___Script___/Animation/{Name}ObjectAnimatorController.cs
   Base: MonoBehaviour + [SerializeField] Animator

DataBuilder
───────────
 • GameDat   → Assets/Game/___Script___/Data/GameDat.cs
   [Serializable] plain C# class

 • GameData  → Assets/Game/___Script___/Data/GameData.cs
   ScriptableObject + [CreateAssetMenu]

SceneBuilder
────────────
 • Add Scene Installer → creates GameObject ""SceneInstaller"" in scene
 • Add General Object  → creates GameObject ""GeneralObject"" in scene

Tip
───
Type only the prefix when prompted.
e.g. typing ""Lobby"" → LobbyPage.cs
in Assets/Game/___Script___/UI/

──────────────────────────────────
Author: Tania Felidae - Game Developer";

        public static void Show()
        {
            var window = GetWindow<ProjectBuilderInfoWindow>(true, "Project Builder Info", true);
            window.minSize = new Vector2(440, 360);
            window.maxSize = new Vector2(440, 360);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            float contentWidth = position.width - 20f; // subtract scrollbar width
            float contentHeight = EditorStyles.wordWrappedLabel.CalcHeight(
                new GUIContent(InfoText), contentWidth);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.SelectableLabel(InfoText, EditorStyles.wordWrappedLabel,
                GUILayout.Height(contentHeight));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Close")) Close();
        }
    }
}
#endif
