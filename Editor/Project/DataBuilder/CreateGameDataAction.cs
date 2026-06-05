#if UNITY_EDITOR
using System.IO;
using UnityEditor.ProjectWindowCallback;

namespace ETEngine.Editor
{
    internal class CreateGameDataAction : EndNameEditAction
    {
        private const string ClassName = "GameData";

        public static void Create()
        {
            Directory.CreateDirectory(ProjectBuilderConstants.DirData);
            string pathName = Path.Combine(ProjectBuilderConstants.DirData, ClassName + ".cs");
            WriteContent(pathName, ClassName);
        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            WriteContent(pathName, Path.GetFileNameWithoutExtension(pathName));
        }

        private static void WriteContent(string pathName, string className)
        {
            string content =
$@"using UnityEngine;

namespace Game
{{
    [CreateAssetMenu(fileName = ""{className}"", menuName = ""Game/Data/{className}"")]
    public class {className} : ScriptableObject
    {{
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }
}
#endif
