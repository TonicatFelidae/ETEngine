#if UNITY_EDITOR
using System.IO;
using UnityEditor.ProjectWindowCallback;

namespace ETEngine.Editor
{
    internal class CreatePopupAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string baseName = Path.GetFileNameWithoutExtension(pathName);
            string className = baseName + ProjectBuilderConstants.SuffixPopup;
            Directory.CreateDirectory(ProjectBuilderConstants.DirUI);
            pathName = $"{ProjectBuilderConstants.DirUI}/{className}.cs";
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
}
#endif
