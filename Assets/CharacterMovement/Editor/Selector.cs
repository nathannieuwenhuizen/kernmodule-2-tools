using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovementClass))]
public class Selector : Editor
{
    static Simulator simulator;
    static readonly float fps = 60;
    static List<Vector3> jumpArc;

    [DrawGizmo(GizmoType.Pickable | GizmoType.Selected)]
    static void DrawGizmosSelected(MovementClass movementClass, GizmoType gizmoType)
    {
        Handles.Label(movementClass.transform.position, "Start point");

        PathCreator.normalJump(movementClass);
        jumpArc = PathCreator.jumpArc(movementClass);
    }


    void OnSceneGUI()
    {
        var character = target as MovementClass;

        Handles.BeginGUI();
        GUI.Box(new Rect(0, 0, 150, 300), "Settings");
        if (GUI.Button(new Rect(25, 225, 100, 30), "Simulate"))
        {
            if (simulator == null)
            {
                simulator = new Simulator();
            }
            simulator.SimulatePath(jumpArc, character.gameObject);
        }
        Handles.EndGUI();

    }
}
