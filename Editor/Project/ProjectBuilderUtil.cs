#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ETEngine.Editor
{
    internal static class ProjectBuilderUtil
    {
        public static void WriteAndImport(string pathName, string content)
        {
            File.WriteAllText(pathName, content);
            AssetDatabase.ImportAsset(pathName);
            ProjectWindowUtil.ShowCreatedAsset(AssetDatabase.LoadAssetAtPath<Object>(pathName));
        }
    }
}
#endif
