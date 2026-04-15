using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ETSimpleKit.SoundSystem
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SoundGeneral))]
    public class SoundGeneralEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SoundGeneral myScript = (SoundGeneral)target;
            DrawDefaultInspector(); if (GUILayout.Button("Init PlayerPrefData"))
            {
                myScript.InitPlayerPrefData();
            }
        }
    }
#endif
}