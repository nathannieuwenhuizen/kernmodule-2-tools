using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CharacterMovementCreator
{
    /// <summary>
    /// For now this is the main editor class
    /// </summary>
    [CustomEditor(typeof(UniqueMovement))]
    public class Selector : Editor
    {
        static Simulator simulator;
        static PlatformPlacer platformPlacer;

        static readonly float fps = 60;


        static List<Vector3> selectedArc;
        static List<Vector3> jumpArc;
        static List<Vector3> doubleJumpArc;

        float hSliderValue = 0f;

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected)]
        static void DrawGizmosSelected(UniqueMovement movementClass, GizmoType gizmoType)
        {
            //draws the simple start point
            Handles.Label(movementClass.transform.position, "Start point");

            //draw idle jump arc
            //PathCreator.DrawIdleJump(movementClass);
            //.DrawDoubleIdleJump(movementClass);

            //draw normal jump arc
            //jumpArc = PathCreator.JumpArc(movementClass);
            //doubleJumpArc = PathCreator.DoubleJumpArc(movementClass);

            if (guibox.selectedSetting == advancedSettings.jump)
            {
                //draw jump arc
                PathCreator.DrawIdleJump(movementClass);
                jumpArc = PathCreator.JumpArc(movementClass);
                selectedArc = jumpArc;

            }
            if (guibox.selectedSetting == advancedSettings.doubleJump)
            {
                PathCreator.DrawDoubleIdleJump(movementClass);
                doubleJumpArc = PathCreator.DoubleJumpArc(movementClass);
                selectedArc = doubleJumpArc;
            }
            if (guibox.selectedSetting == advancedSettings.dash)
            {

            }
            if (guibox.selectedSetting == advancedSettings.crouch)
            {

            }

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
            DrawDefaultInspector();
            UniqueMovement character = (UniqueMovement)target;

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


            //DrawDefaultInspector();
            //var levelScript = target as LevelScript;
            //levelScript.experience = EditorGUILayout.IntField("Experience", levelScript.experience);
            //EditorGUILayout.LabelField("Level", levelScript.Level + "");
            //serializedObject.ApplyModifiedProperties();
        }

        static public MyGUIBox guibox;

        public float vSbarValue;
        void OnSceneGUI()
        {
            var character = target as UniqueMovement;

            if (guibox == null)
            {
                guibox = new MyGUIBox();
            }
            guibox.Draw(character);
            guibox.simulating = simulating;
            guibox.spawning = platformSpawning;

            var transform = character.transform;
            if (guibox.selectedSetting == advancedSettings.jump)
            {
                using (var cc = new EditorGUI.ChangeCheckScope())
                {

                    Vector3 jumpHandeler =
                    Handles.PositionHandle(
                        transform.position + new Vector3(PathCreator.xJumpDistance / 2, PathCreator.GetJumpUnits(character), 0),
                        transform.rotation);
                    if (cc.changed)
                    {
                        character.jumpSpeed = PathCreator.SetJumpUnits(jumpHandeler.y - character.transform.position.y, character);
                        changeWalkSpeed(character, Mathf.Max((jumpHandeler.x - transform.position.x), 0));
                    }
                }
            }
            if (guibox.selectedSetting == advancedSettings.doubleJump)
            {
                using (var cc = new EditorGUI.ChangeCheckScope())
                {

                    Vector3 doubleJumpHandeler =
                    Handles.PositionHandle(
                        transform.position + new Vector3(0, PathCreator.GetDoubleJumpUnits(character), 0),
                        transform.rotation);
                    if (cc.changed)
                    {
                        character.doubleJumpSpeed = PathCreator.SetJumpUnits(doubleJumpHandeler.y - (character.transform.position.y + PathCreator.GetJumpUnits(character)), character);
                    }
                }
            }
        }
        public void simulating()
        {
            if (simulator == null)
            {
                simulator = new Simulator();
            }
            var character = target as UniqueMovement;
            if (selectedArc != null)
            {
                simulator.SimulatePath(selectedArc, character.gameObject);
            }
        }
        public void platformSpawning()
        {
            if (platformPlacer == null)
            {
                platformPlacer = new PlatformPlacer();
            }
            var character = target as UniqueMovement;
            List<Vector3> positions = new List<Vector3> { character.transform.position };
            positions.Add(selectedArc[selectedArc.Count - 1]);
            platformPlacer.PlacePlatformObjects(positions, character.GetComponent<BoxCollider2D>());
        }

        //changes the walkspeed of the movement by selection
        public void changeWalkSpeed(UniqueMovement character, float handlerPos)
        {
            //if handler isnt at the origin of the character transform
            if (handlerPos != 0) {
                //if the walkspeed were 0,  so skip some breaking calculations!
                if (character.walkSpeed == 0) {
                    character.walkSpeed = (handlerPos * 2) / 0.001f; //0.0001f is just a really small number to prevent infinity calculations
                }
                //calculate walkspeed as normal
                else {
                    character.walkSpeed = (handlerPos * 2) / PathCreator.xJumpDistance * character.walkSpeed;
                }
            }
            else {

                character.walkSpeed = 0;
            }
            PathCreator.xJumpDistance = handlerPos;
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
}
