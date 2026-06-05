#if UNITY_EDITOR
using System.IO;
using UnityEditor.ProjectWindowCallback;

namespace ETEngine.Editor
{
    internal class CreateInstallerAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string baseName = Path.GetFileNameWithoutExtension(pathName);
            string className = baseName + ProjectBuilderConstants.SuffixInstaller;
            Directory.CreateDirectory(ProjectBuilderConstants.DirInstaller);
            pathName = Path.Combine(ProjectBuilderConstants.DirInstaller, className + ".cs");
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
}
#endif
