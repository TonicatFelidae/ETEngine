#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace ET
{
    public static class NewGameMonoBehaviourScriptEditor
    {
        [MenuItem("Assets/Create/Scripting/GameMonoBehaviourScript", false, 80)]
        private static void CreateGameMonoBehaviourScript()
        {
            string path = GetSelectedPathOrFallback();
            string filePath = Path.Combine(path, "NewGameMonoBehaviourScript.cs");

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<CreateGameMonoBehaviourAction>(),
                filePath,
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                null
            );
        }

        private static string GetSelectedPathOrFallback()
        {
            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (Directory.Exists(path))
                    return path;
                if (File.Exists(path))
                    return Path.GetDirectoryName(path);
            }
            return "Assets";
        }
    }

    internal class CreateGameMonoBehaviourAction : EndNameEditAction
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

            File.WriteAllText(pathName, content);
            AssetDatabase.ImportAsset(pathName);
            ProjectWindowUtil.ShowCreatedAsset(
                AssetDatabase.LoadAssetAtPath<Object>(pathName)
            );
        }
    }

    public static class NewGameScriptEditor
    {
        [MenuItem("Assets/Create/Scripting/GameScript", false, 81)]
        private static void CreateGameScript()
        {
            string path = GetSelectedPathOrFallback();
            string filePath = Path.Combine(path, "NewGameScript.cs");

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<CreateGameScriptAction>(),
                filePath,
                EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D,
                null
            );
        }

        private static string GetSelectedPathOrFallback()
        {
            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (Directory.Exists(path))
                    return path;
                if (File.Exists(path))
                    return Path.GetDirectoryName(path);
            }
            return "Assets";
        }
    }

    internal class CreateGameScriptAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string className = Path.GetFileNameWithoutExtension(pathName);

            string content =
$@"using UnityEngine;

namespace Game
{{
    public class {className}
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

            File.WriteAllText(pathName, content);
            AssetDatabase.ImportAsset(pathName);
            ProjectWindowUtil.ShowCreatedAsset(
                AssetDatabase.LoadAssetAtPath<Object>(pathName)
            );
        }
    }

}
#endif