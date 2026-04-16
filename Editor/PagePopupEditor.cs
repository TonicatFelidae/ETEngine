#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public static class NewPageScriptEditor
{
    [MenuItem("Assets/Create/Scripting/NewPage", false, 82)]
    private static void CreateNewPageScript()
    {
        string path = GetSelectedPathOrFallback();
        string filePath = Path.Combine(path, "NewPage.cs");

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<CreateNewPageScriptAction>(),
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

internal class CreateNewPageScriptAction : EndNameEditAction
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

        File.WriteAllText(pathName, content);
        AssetDatabase.ImportAsset(pathName);
        ProjectWindowUtil.ShowCreatedAsset(
            AssetDatabase.LoadAssetAtPath<Object>(pathName)
        );
    }
}

public static class NewPopupScriptEditor
{
    [MenuItem("Assets/Create/Scripting/NewPopup", false, 83)]
    private static void CreateNewPopupScript()
    {
        string path = GetSelectedPathOrFallback();
        string filePath = Path.Combine(path, "NewPopup.cs");

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<CreateNewPopupScriptAction>(),
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

internal class CreateNewPopupScriptAction : EndNameEditAction
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

        File.WriteAllText(pathName, content);
        AssetDatabase.ImportAsset(pathName);
        ProjectWindowUtil.ShowCreatedAsset(
            AssetDatabase.LoadAssetAtPath<Object>(pathName)
        );
    }
}

#endif
