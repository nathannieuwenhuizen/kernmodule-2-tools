using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterWindow : EditorWindow
{

    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;
    ClassData data = new ClassData();
    public void Awake()
    {
        Debug.Log("awake");

    }

    // Add menu named "My Window" to the Window menu
    [MenuItem("CharacterMovement/Settings")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CharacterWindow window = (CharacterWindow)EditorWindow.GetWindow(typeof(CharacterWindow));
        window.Show();

    }

    void OnGUI()
    {
        GUILayout.Label("File Settings", EditorStyles.boldLabel);
        data.className = EditorGUILayout.TextField("Class name", data.className);
        //folder setting
        if (GUILayout.Button("Path: " + data.path.Replace(Application.dataPath, "Assets")))
        {
            data.path = EditorUtility.SaveFolderPanel("Save textures to folder", "", "");
        }

        //variable settings
        GUILayout.Label("Movement Settings", EditorStyles.boldLabel);
        data.walkspeed = EditorGUILayout.Slider("Walk speed", data.walkspeed, 0, 100);
        data.friction = EditorGUILayout.Slider("Friction", data.friction, 0, 1);
        data.jumpSpeed = EditorGUILayout.Slider("Jump speed", data.jumpSpeed, 0, 100);
        data.gravityScale = EditorGUILayout.Slider("Gravity scale", data.gravityScale, 0, 1);
        data.maxFallSpeed = EditorGUILayout.Slider("Max fall speed", data.maxFallSpeed, 0, 100);
        data.justInTimeDurationOnGround = EditorGUILayout.Slider("Just in time on ground", data.justInTimeDurationOnGround, 0, 3);
        

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();


        //generate
        if (GUILayout.Button("Generate script"))
        {
            CharacterMaker.GenerateScript(data);
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Generate script with object"))
        {
            CharacterMaker.GenerateScript(data, true);
            AssetDatabase.Refresh();
        }

    }
}
