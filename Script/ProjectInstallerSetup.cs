using System;
using UnityEngine;

namespace ETEngine
{
    [Serializable]
    [CreateAssetMenu(fileName = "ProjectInstallerSetup", menuName = "ETEngine/ProjectInstallerSetup")]
    public class ProjectInstallerSetup : ScriptableObject
    {
        public bool useMainCanvasForAll;
        public MainCanvas mainCanvas;
    }
}