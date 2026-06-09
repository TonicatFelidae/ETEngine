#if UNITY_EDITOR
using System.IO;
using UnityEditor.ProjectWindowCallback;

namespace ETEngine.Editor
{
    internal class CreatePageAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string baseName = Path.GetFileNameWithoutExtension(pathName);
            string className = baseName + ProjectBuilderConstants.SuffixPage;
            Directory.CreateDirectory(ProjectBuilderConstants.DirUI);
            pathName = $"{ProjectBuilderConstants.DirUI}/{className}.cs";
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
}
#endif
