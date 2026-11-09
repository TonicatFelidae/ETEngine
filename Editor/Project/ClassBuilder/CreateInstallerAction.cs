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
            string className = baseName.EndsWith(ProjectBuilderConstants.SuffixInstaller)
                ? baseName
                : baseName + ProjectBuilderConstants.SuffixInstaller;
            Directory.CreateDirectory(ProjectBuilderConstants.DirInstaller);
            pathName = Path.Combine(ProjectBuilderConstants.DirInstaller, className + ".cs");
            string content =
$@"using ETEngine;
using UnityEngine;
using VContainer;

namespace Game
{{
    public class {className} : ETLifetimeScope
    {{
        protected override void Register(IContainerBuilder builder)
        {{
            base.Configure(builder);
        }}

        protected override void RegisterBuildCallback(IObjectResolver resolver)
        {{
            base.RegisterBuildCallback(resolver);
        }}
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }
}
#endif
