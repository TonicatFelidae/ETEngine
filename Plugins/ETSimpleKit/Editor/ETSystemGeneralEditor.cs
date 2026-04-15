using ETSimpleKit.SoundSystem;
using UnityEditor;
using UnityEngine;

namespace ETSimpleKit
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ETSystemGeneral))]
    public class ETSystemGeneralEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ETSystemGeneral myScript = (ETSystemGeneral)target;
            DrawDefaultInspector();
            if (GUILayout.Button("Init PlayerPrefData"))
            {
                myScript.InitPlayerPrefData();
            }
        }
    }
#endif  
}