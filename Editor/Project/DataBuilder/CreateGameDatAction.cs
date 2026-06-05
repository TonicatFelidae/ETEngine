#if UNITY_EDITOR
using System.IO;
using UnityEditor.ProjectWindowCallback;

namespace ETEngine.Editor
{
    internal class CreateGameDatAction : EndNameEditAction
    {
        private const string ClassName = "GameDat";

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
$@"using System;

namespace Game
{{
    public class {className}
    {{
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }
}
#endif
