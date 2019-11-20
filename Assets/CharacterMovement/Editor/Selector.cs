using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// For now this is the main editor class
/// </summary>
/// 

//custom editor for a movementclass
[CustomEditor(typeof(MovementClass))]
public class Selector : Editor
{
    static Simulator simulator;
    static PlatformPlacer platformPlacer;

    static readonly float fps = 60;

    static List<Vector3> jumpArc;

    float hSliderValue = 0f;

    [DrawGizmo(GizmoType.Pickable | GizmoType.Selected)]
    static void DrawGizmosSelected(MovementClass movementClass, GizmoType gizmoType)
    {
        //draws the simple start point
        Handles.Label(movementClass.transform.position, "Start point");

        //draw idle jump arc
        PathCreator.DrawIdleJump(movementClass);

        //draw normal jump arc
        jumpArc = PathCreator.JumpArc(movementClass);

    }

    ToggleMenu jumpMenu;
    ToggleMenu walkMenu;

    void GuiLine(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        MovementClass character = (MovementClass)target;

        //EditorGUILayout.IntSlider(0, 0, 18);

        GUILayout.Label("Movement options");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Gravity");
        character.gravityScale = EditorGUILayout.Slider(character.gravityScale, 0.1f, 10f);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Max fall speed");
        character.maxFallSpeed = EditorGUILayout.Slider(character.maxFallSpeed, 1, 50f);
        GUILayout.EndHorizontal();

        GUILayout.Label("General info");

        if (walkMenu == null)
        {
            walkMenu = new ToggleMenu("Walk");
        }
        walkMenu.Header();
        if (walkMenu.foldout)
        {
            EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Walk speed");
            character.walkSpeed = EditorGUILayout.Slider(character.walkSpeed, 0, 50f);
            GUILayout.EndHorizontal();
            //GUILayout.BeginHorizontal();
            //GUILayout.Label("Friction");
            //character.gravityScale = EditorGUILayout.Slider(character.walkSpeed, 0.1f, 10f); //TODO: make friction into script
            //GUILayout.EndHorizontal();
            EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
        }
        walkMenu.Footer();

        if (jumpMenu == null)
        {
            jumpMenu = new ToggleMenu("Jump");
        }
        jumpMenu.Header();
        if (jumpMenu.foldout)
        {
            EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Jump speed");
            character.jumpSpeed = EditorGUILayout.Slider(character.jumpSpeed, 0, 50f);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Off edge jump duration");
            character.justInTimeDurationOnGround = EditorGUILayout.Slider(character.justInTimeDurationOnGround, 0, 1f);
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
        }
        jumpMenu.Footer();

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



    public float vSbarValue;
    void OnSceneGUI()
    {
        var character = target as MovementClass;

        Handles.BeginGUI();

        GUI.Box(new Rect(0, 0, 150, 300), "Settings");

        //vSbarValue = GUI.VerticalScrollbar(new Rect(0, 20, 150, 300), vSbarValue, 1.0F, 10.0F, 0.0F);
        if (GUI.Button(new Rect(25, 225, 100, 30), "Simulate"))
        {
            if (simulator == null)
            {
                simulator = new Simulator();
            }
            simulator.SimulatePath(jumpArc, character.gameObject);
        }
        if (GUI.Button(new Rect(25, 180, 100, 30), "Place platforms"))
        {
            if (platformPlacer == null)
            {
                platformPlacer = new PlatformPlacer();
            }

            List<Vector3> positions = new List<Vector3> { character.transform.position };
            positions.Add(jumpArc[jumpArc.Count - 1]);
            platformPlacer.PlacePlatformObjects(positions, character.GetComponent<BoxCollider2D>());
        }


        hSliderValue = GUI.HorizontalSlider(new Rect(25, 25, 100, 30), hSliderValue, 0.0F, 10.0F);
        EditorGUIUtility.AddCursorRect(new Rect(20, 20, 140, 40), MouseCursor.Zoom);
        Handles.EndGUI();


        var transform = character.transform;

        using (var cc = new EditorGUI.ChangeCheckScope())
        {
            //Vector3 jumpHandler =
            //Handles.PositionHandle(
            //    transform.position + new Vector3(0, PathCreator.GetJumpUnits(character), 0),
            //    transform.rotation);

            //Vector3 walkHandeler =
            //Handles.PositionHandle(
            //    transform.position + new Vector3( PathCreator.xDistance, 0, 0),
            //    transform.rotation);

            Vector3 realJumpHandeler =
            Handles.PositionHandle(
                transform.position + new Vector3(PathCreator.xDistance / 2, PathCreator.GetJumpUnits(character), 0),
                transform.rotation);

            if (cc.changed)
            {

                //character.jumpSpeed = PathCreator.SetJumpUnits(jumpHandler.y - character.transform.position.y, character);
                //float handlerPos = (walkHandeler.x - transform.position.x); 

                character.jumpSpeed = PathCreator.SetJumpUnits(realJumpHandeler.y - character.transform.position.y, character);
                changeWalkSpeed(character, Mathf.Max((realJumpHandeler.x - transform.position.x), 0));

            }
        }
    }

    //changes the walkspeed of the movement by selection
    public void changeWalkSpeed(MovementClass character, float handlerPos)
    {
        //if handler isnt at the origin of the character transform
        if (handlerPos != 0) {
            //if the walkspeed were 0,  so skip some breaking calculations!
            if (character.walkSpeed == 0) {
                character.walkSpeed = (handlerPos * 2) / 0.001f; //0.0001f is just a really small number to prevent infinity calculations
            }
            //calculate walkspeed as normal
            else {
                character.walkSpeed = (handlerPos * 2) / PathCreator.xDistance * character.walkSpeed;
            }
        }
        else {

            character.walkSpeed = 0;
        }
        PathCreator.xDistance = handlerPos;
    }
}
public class ToggleMenu
{
    public bool foldout = false;
    public bool enabled = true;
    public string name;
    public ToggleMenu(string _name)
    {
        name = _name;
    }
    public void Header()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        foldout = EditorGUILayout.Foldout(foldout, name);
        enabled = EditorGUILayout.Toggle(enabled);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GuiLine();
    }
    public void Footer()
    {
        if (foldout && enabled)
        {
            GuiLine();
        }
    }
    void GuiLine(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

}