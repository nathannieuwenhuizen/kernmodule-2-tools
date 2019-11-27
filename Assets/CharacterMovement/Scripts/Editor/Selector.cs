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
        static List<Vector3> walkArc;

        [DrawGizmo(GizmoType.Pickable | GizmoType.Selected)]
        static void DrawGizmosSelected(UniqueMovement movementClass, GizmoType gizmoType)
        {
            //draws the simple start point
            Handles.Label(movementClass.transform.position, "Start point");

            if (guibox.selectedSetting == advancedSettings.general)
            {
                selectedArc = PathCreator.WalkArc(movementClass);
            }
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
                selectedArc = PathCreator.DashArc(movementClass);
            }
            if (guibox.selectedSetting == advancedSettings.crouch)
            {
                selectedArc = PathCreator.CrouchArc(movementClass);
            }

        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        static public MyGUIBox guibox;

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
                        character.jumpSpeed = Mathf.Max(1f, PathCreator.SetJumpUnits(jumpHandeler.y - character.transform.position.y, character));
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
                        character.doubleJumpSpeed = Mathf.Max(0, PathCreator.SetJumpUnits(doubleJumpHandeler.y - (character.transform.position.y + PathCreator.GetJumpUnits(character)), character));
                    }
                }
            }
            if (guibox.selectedSetting == advancedSettings.general)
            {
                using (var cc = new EditorGUI.ChangeCheckScope())
                {

                    Vector3 walkHandeler =
                    Handles.PositionHandle(
                        transform.position + new Vector3(PathCreator.xWalkDistance, 0, 0),
                        transform.rotation);
                    if (cc.changed)
                    {
                        character.walkSpeed = PathCreator.SetWalkUnits(walkHandeler.x - character.transform.position.x, character);
                    }
                }
            }
            if (guibox.selectedSetting == advancedSettings.dash)
            {
                using (var cc = new EditorGUI.ChangeCheckScope())
                {

                    Vector3 dashHandeler =
                    Handles.PositionHandle(
                        transform.position + new Vector3(PathCreator.xDashDistance, 0, 0),
                        transform.rotation);
                    if (cc.changed)
                    {
                        character.dashSpeed = Mathf.Max(0, PathCreator.SetDashUnits(dashHandeler.x - character.transform.position.x, character));
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


            float height = character.GetComponent<BoxCollider2D>().size.y * character.transform.localScale.y;
            float width = character.GetComponent<BoxCollider2D>().size.x * character.transform.localScale.x;
            float platformHeight = .5f;

            if (guibox.selectedSetting == advancedSettings.jump || guibox.selectedSetting == advancedSettings.doubleJump)
            {
                //begin and end platform
                platformPlacer.PlaceGround(character.transform.position + new Vector3(0, -height / 2 + platformHeight / 2), new Vector2(width * 2, platformHeight));
                platformPlacer.PlaceGround(selectedArc[selectedArc.Count - 1] + new Vector3(0, -height / 2 + platformHeight / 2), new Vector2(width * 2, platformHeight));
            } else if (guibox.selectedSetting == advancedSettings.general)
            {
                //global ground
                platformPlacer.PlaceGround(selectedArc[(int)Mathf.Floor(selectedArc.Count / 2)] + new Vector3(0, -height / 2 + platformHeight / 2), new Vector2((selectedArc[selectedArc.Count -1].x - selectedArc[0].x) + width * 2, platformHeight));
            }
            else if (guibox.selectedSetting == advancedSettings.crouch)
            {
                //global ground
                platformPlacer.PlaceGround(selectedArc[(int)Mathf.Floor(selectedArc.Count / 2)] + new Vector3(0, -height / 2 + platformHeight / 2), new Vector2((selectedArc[selectedArc.Count - 1].x - selectedArc[0].x) + width * 2, 0.5f));

                //ceil
                platformPlacer.PlaceGround(selectedArc[(int)Mathf.Floor(selectedArc.Count / 2)] + new Vector3(0, height / 2 * character.crouchScale + platformHeight), new Vector2((selectedArc[selectedArc.Count - 1].x - selectedArc[0].x) / 2f, platformHeight));
            }
            else if (guibox.selectedSetting == advancedSettings.dash)
            {
                //begin and end platform
                platformPlacer.PlaceGround(character.transform.position + new Vector3(0, -height / 2 + platformHeight / 2), new Vector2(width * 2, platformHeight));
                platformPlacer.PlaceGround(selectedArc[selectedArc.Count - 1] + new Vector3(0, -height / 2 + platformHeight / 2), new Vector2(width * 2, platformHeight));

                platformPlacer.PlaceGround(selectedArc[(int)Mathf.Floor(selectedArc.Count / 2)] + new Vector3(0, height / 2 + platformHeight * 2f), new Vector2((selectedArc[selectedArc.Count - 1].x - selectedArc[0].x) + width * 2, platformHeight));
            }
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
}
