using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuItems
{
    [MenuItem("Tools/Clear PlayerPrefs _g")]
    private static void NewMenuOption()
    {
        PlayerPrefs.DeleteAll();
        Debug.LogError("HAHAHAHA!");
    }

    [MenuItem("Assets/Load Additive Scene")]
    private static void LoadAdditiveScene()
    {
        var selected = Selection.activeObject;
        EditorApplication.OpenSceneAdditive(AssetDatabase.GetAssetPath(selected));
    }

    [MenuItem("CONTEXT/Rigidbody/New Option")]
    private static void NewOpenForRigidBody()
    {
        Debug.LogError("HAHAHAHA!");
    }
}
