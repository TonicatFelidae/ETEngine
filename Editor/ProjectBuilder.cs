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
            if (GUILayout.Button("Add Scene Installer")) AddSceneInstaller();
            if (GUILayout.Button("Add General Object")) AddGeneralObject();
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
            StartNameEdit<CreateGeneralObjectAction>("NewGeneralObject.cs");
        }

        private static void CreatePage()
        {
            StartNameEdit<CreatePageAction>("NewPage.cs");
        }

        private static void CreatePopup()
        {
            StartNameEdit<CreatePopupAction>("NewPopup.cs");
        }

        private static void CreateSheet()
        {
            StartNameEdit<CreateSheetAction>("NewSheet.cs");
        }

        private static void CreateInstaller()
        {
            StartNameEdit<CreateInstallerAction>("NewInstaller.cs");
        }

        private static void CreateObjectAnimatorController()
        {
            StartNameEdit<CreateObjectAnimatorControllerAction>("NewObjectAnimatorController.cs");
        }

        // ── Data ─────────────────────────────────────────────────────────────

        private static void CreateGameDat()
        {
            StartNameEdit<CreateGameDatAction>("NewGameDat.cs");
        }

        private static void CreateGameData()
        {
            StartNameEdit<CreateGameDataAction>("NewGameData.cs");
        }

        // ── SceneBuilder ─────────────────────────────────────────────────────

        private static void AddSceneInstaller()
        {
            var go = new GameObject("SceneInstaller");
            Undo.RegisterCreatedObjectUndo(go, "Add SceneInstaller");
            Selection.activeGameObject = go;
        }

        private static void AddGeneralObject()
        {
            var go = new GameObject("GeneralObject");
            Undo.RegisterCreatedObjectUndo(go, "Add GeneralObject");
            Selection.activeGameObject = go;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static void StartNameEdit<T>(string defaultFileName) where T : EndNameEditAction
        {
            string path = GetSelectedPathOrFallback();
            string filePath = Path.Combine(path, defaultFileName);
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

    // ── EndNameEditActions ────────────────────────────────────────────────────

    internal class CreateGeneralObjectAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);
            string content =
$@"using ETEngine;
using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using System;

namespace Game
{{
    public class {className} : MonoBehaviour, ISceneEntryPoint
    {{
        ClassInitState _classInitState = ClassInitState.UnInitialized;

        async void Start()
        {{
            if (_classInitState == ClassInitState.UnInitialized)
            {{
                _classInitState = ClassInitState.Initializing;
                await Init(new Progress<float>());
                _classInitState = ClassInitState.Initialized;
            }}
        }}

        void Update()
        {{
        }}

        public async UniTask Init(IProgress<float> progress)
        {{
            _classInitState = ClassInitState.Initializing;
            _classInitState = ClassInitState.Initialized;
        }}
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }

    internal class CreatePageAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);
            string content =
$@"using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Page;

namespace Game.UI
{{
    public class {className} : Page
    {{
        private void Awake()
        {{
        }}

        private void Start()
        {{
        }}

        private void Init()
        {{
        }}

        private void Update()
        {{
        }}
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }

    internal class CreatePopupAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);
            string content =
$@"using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Modal;

namespace Game.UI
{{
    public class {className} : Popup
    {{
        private void Awake()
        {{
        }}

        private void Start()
        {{
        }}

        private void Init()
        {{
        }}

        private void Update()
        {{
        }}
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }

    internal class CreateSheetAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);
            string content =
$@"using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Sheet;

namespace Game.UI
{{
    public class {className} : Sheet
    {{
        private void Awake()
        {{
        }}

        private void Start()
        {{
        }}

        private void Init()
        {{
        }}

        private void Update()
        {{
        }}
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }

    internal class CreateInstallerAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);
            string content =
$@"using UnityEngine;

namespace Game
{{
    public class {className} : MonoBehaviour
    {{
        private void Awake()
        {{
        }}

        private void Start()
        {{
        }}

        private void Update()
        {{
        }}
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }

    internal class CreateObjectAnimatorControllerAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);
            string content =
$@"using UnityEngine;

namespace Game
{{
    public class {className} : MonoBehaviour
    {{
        [SerializeField] private Animator _animator;

        private void Awake()
        {{
        }}

        private void Start()
        {{
        }}

        private void Update()
        {{
        }}
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }

    internal class CreateGameDatAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);
            string content =
$@"using System;

namespace Game.Data
{{
    [Serializable]
    public class {className}
    {{
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }

    internal class CreateGameDataAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);
            string content =
$@"using UnityEngine;

namespace Game.Data
{{
    [CreateAssetMenu(fileName = ""{className}"", menuName = ""Game/Data/{className}"")]
    public class {className} : ScriptableObject
    {{
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }

    // ── Shared utility ────────────────────────────────────────────────────────

    internal static class ProjectBuilderUtil
    {
        public static void WriteAndImport(string pathName, string content)
        {
            File.WriteAllText(pathName, content);
            AssetDatabase.ImportAsset(pathName);
            ProjectWindowUtil.ShowCreatedAsset(AssetDatabase.LoadAssetAtPath<Object>(pathName));
        }
    }
}
#endif