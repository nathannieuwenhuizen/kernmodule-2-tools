using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelScript))]
public class LevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelScript myScript = (LevelScript)target;
        bool build = GUILayout.Button("Build Object");
        if (build)
        {
            myScript.BuildObject();
        }

        //serializedObject.Update();

        //DrawDefaultInspector();
        //var levelScript = target as LevelScript;
        //levelScript.experience = EditorGUILayout.IntField("Experience", levelScript.experience);
        //EditorGUILayout.LabelField("Level", levelScript.Level + "");
        //serializedObject.ApplyModifiedProperties();
    }
    public void OnEnable()
    {


    }
}
