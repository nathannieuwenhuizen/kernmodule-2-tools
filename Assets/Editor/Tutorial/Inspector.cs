using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementClass))]
public class Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        MovementClass myScript = (MovementClass)target;

        bool build = GUILayout.Button("Build Object");
        if (build)
        {
            //myScript.BuildObject();
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

