#if UNITY_EDITOR
using System.IO;
using UnityEditor.ProjectWindowCallback;

namespace ETEngine.Editor
{
    internal class CreateObjectAnimatorControllerAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string baseName = Path.GetFileNameWithoutExtension(pathName);
            string className = baseName + ProjectBuilderConstants.SuffixObjectAnimatorController;
            Directory.CreateDirectory(ProjectBuilderConstants.DirAnimation);
            pathName = Path.Combine(ProjectBuilderConstants.DirAnimation, className + ".cs");
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
}
#endif
