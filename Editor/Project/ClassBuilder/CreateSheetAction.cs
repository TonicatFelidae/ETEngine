#if UNITY_EDITOR
using System.IO;
using UnityEditor.ProjectWindowCallback;

namespace ETEngine.Editor
{
    internal class CreateSheetAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string baseName = Path.GetFileNameWithoutExtension(pathName);
            string className = baseName + ProjectBuilderConstants.SuffixSheet;
            Directory.CreateDirectory(ProjectBuilderConstants.DirUI);
            pathName = Path.Combine(ProjectBuilderConstants.DirUI, className + ".cs");
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
}
#endif
