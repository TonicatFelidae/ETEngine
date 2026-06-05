#if UNITY_EDITOR
using System.IO;
using UnityEditor.ProjectWindowCallback;

namespace ETEngine.Editor
{
    internal class CreateGeneralObjectAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string baseName = Path.GetFileNameWithoutExtension(pathName);
            string className = baseName + ProjectBuilderConstants.SuffixGeneralObject;
            Directory.CreateDirectory(ProjectBuilderConstants.DirGeneralObject);
            pathName = Path.Combine(ProjectBuilderConstants.DirGeneralObject, className + ".cs");
            string content =
$@"using ETEngine;
using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using System;

namespace Game
{{
    public class {className} : MonoBehaviour, ISceneEntryPoint
    {{
        ClassInitState _classInitState = ClassInitState.UnInitialized;

        async void Start()
        {{
            if (_classInitState == ClassInitState.UnInitialized)
            {{
                _classInitState = ClassInitState.Initializing;
                await Init(new Progress<float>());
                _classInitState = ClassInitState.Initialized;
            }}
        }}

        void Update()
        {{
        }}

        public async UniTask Init(IProgress<float> progress)
        {{
            _classInitState = ClassInitState.Initializing;
            _classInitState = ClassInitState.Initialized;
        }}
    }}
}}";
            ProjectBuilderUtil.WriteAndImport(pathName, content);
        }
    }
}
#endif
