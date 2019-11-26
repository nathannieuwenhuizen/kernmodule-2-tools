﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CharacterMovementCreator
{
    public enum advancedSettings
    {
        general,
        jump,
        doubleJump,
        crouch,
        dash,
    }

    public class MyGUIBox
    {
        public float hSliderValue;
        public Vector2 pos;
        public Vector2 size;

        public delegate void simulateEvent();
        public simulateEvent simulating;
        public delegate void spawnEvent();
        public spawnEvent spawning;
        public GUIStyle style;
        public float horizontalOffset = 5;

        public advancedSettings selectedSetting = advancedSettings.general;

        //round to decimals
        static float RoundToDecimals(float val, int amount)
        {
            float pow = Mathf.Pow(10, amount);
            return Mathf.Round(val * pow) / pow;
        }

        public float valueField(float yPos, string header, float val, float min, float max)
        {
            GUI.Label(new Rect(pos.x + horizontalOffset, yPos, size.x - horizontalOffset * 2, 15), header + " " + RoundToDecimals(val, 2));
            float value = GUI.HorizontalSlider(new Rect(pos.x + horizontalOffset, yPos + 10, size.x - horizontalOffset * 2, 15), val, min, max);
            EditorGUIUtility.AddCursorRect(new Rect(pos.x + horizontalOffset, yPos + 10, size.x - horizontalOffset * 2, 15), MouseCursor.Link);
            return value;
        }

        public int valueField(float yPos, string header, int val, int min, int max)
        {
            GUI.Label(new Rect(pos.x + horizontalOffset, yPos, size.x - horizontalOffset * 2, 15), header);
            int value = EditorGUI.IntSlider(new Rect(pos.x + horizontalOffset, yPos + 10, size.x - horizontalOffset * 2, 15), val, min, max);
            EditorGUIUtility.AddCursorRect(new Rect(pos.x + horizontalOffset, yPos + 10, size.x - horizontalOffset * 2, 15), MouseCursor.Link);
            return value;
        }
        public void Draw(UniqueMovement character)
        {
            float lineSpace = 15;
            pos = new Vector2(5, 5);
            size = new Vector2(150, 300);

            Handles.BeginGUI();

            GUI.color = new Color(.8f, 1.0f, .8f, 0.9f);
            GUI.Box(new Rect(pos.x, pos.y, size.x, size.y), "Settings");
            GUI.color = Color.white;
            float currentPos = pos.y;

            currentPos += lineSpace;
            character.jumpEnabled = GUI.Toggle(new Rect(pos.x + horizontalOffset, currentPos, size.x, 15), character.jumpEnabled, "jump");
            currentPos += lineSpace;
            character.doubleJumpEnabled = GUI.Toggle(new Rect(pos.x + horizontalOffset, currentPos, size.x, 15), character.doubleJumpEnabled, "double jump");
            currentPos += lineSpace;
            character.enableDash = GUI.Toggle(new Rect(pos.x + horizontalOffset, currentPos, size.x, 15), character.enableDash, "dash");
            currentPos += lineSpace;
            character.crouchEnabled = GUI.Toggle(new Rect(pos.x + horizontalOffset, currentPos, size.x, 15), character.crouchEnabled, "crouch");
            currentPos += lineSpace;

            GUI.Label(new Rect(pos.x + horizontalOffset, currentPos, size.x - horizontalOffset * 2, 15), "", GUI.skin.horizontalSlider);
            currentPos += 30;
            GUI.Label(new Rect(pos.x + horizontalOffset, currentPos, size.x, 15), "Advanced settings");
            currentPos += lineSpace;

            GUI.backgroundColor = Color.yellow;
            selectedSetting = (advancedSettings)EditorGUI.EnumPopup(new Rect(pos.x + horizontalOffset * 2, currentPos, size.x - horizontalOffset * 4, 30), selectedSetting);
            GUI.backgroundColor = Color.white;

            GUI.skin.label.normal.textColor = new Color(0.2f, 0.2f, 0f);

            currentPos += 20;
            switch (selectedSetting)
            {
                case advancedSettings.general:
                    character.walkSpeed = valueField(currentPos, "walk speed", character.walkSpeed, 0, 50f);
                    currentPos += 25;
                    character.faceToDirection = GUI.Toggle(new Rect(pos.x + horizontalOffset, currentPos, size.x, 15), character.faceToDirection, "Faces direction");
                    break;
                case advancedSettings.jump:
                    EditorGUI.BeginDisabledGroup(!character.jumpEnabled);
                    character.gravityScale = valueField(currentPos, "gravity", character.gravityScale, 0, 2f);
                    currentPos += 20;
                    character.maxFallSpeed = valueField(currentPos, "max fall speed", character.maxFallSpeed, 1, 200f);
                    currentPos += 20;
                    character.justInTimeDurationOnGround = valueField(currentPos, "Off edge duration", character.justInTimeDurationOnGround, 0, .5f);
                    currentPos += 20;
                    EditorGUI.EndDisabledGroup();
                    break;
                case advancedSettings.doubleJump:
                    EditorGUI.BeginDisabledGroup(!character.doubleJumpEnabled);
                    character.doubleJumpSpeed = valueField(currentPos, "double jump speed", character.doubleJumpSpeed, 0, 50f);
                    currentPos += 25;
                    character.amountOfDoubleJumps = valueField(currentPos, "amount of jumps", character.amountOfDoubleJumps, 1, 10);
                    EditorGUI.EndDisabledGroup();

                    break;
                case advancedSettings.dash:
                    EditorGUI.BeginDisabledGroup(!character.enableDash);
                    character.dashSpeed = valueField(currentPos, "dash speed", character.dashSpeed, 0, 100f);
                    currentPos += 20;
                    character.dashDuration = valueField(currentPos, "dash duration", character.dashDuration, 0.01f, 1f);
                    currentPos += 25;

                    GUI.Label(new Rect(pos.x + horizontalOffset, currentPos, size.x, 15), "Dash style");
                    currentPos += 15;
                    character.dashMode = (dashModes)EditorGUI.EnumPopup(new Rect(pos.x + horizontalOffset, currentPos, size.x / 2, 30), character.dashMode);

                    EditorGUI.EndDisabledGroup();

                    break;
                case advancedSettings.crouch:
                    EditorGUI.BeginDisabledGroup(!character.crouchEnabled);
                    character.crouchSpeed = valueField(currentPos, "crouch speed", character.crouchSpeed, 0, 20f);
                    currentPos += 20;
                    character.crouchScale = valueField(currentPos, "crouch size scale", character.crouchScale, 0.01f, 1f);
                    currentPos += 20; 

                    EditorGUI.EndDisabledGroup();

                    break;
                default:
                    break;
            }
            GUI.skin.button.normal.textColor = Color.black;

            GUI.Label(new Rect(pos.x + horizontalOffset, pos.y + size.y - 70, size.x - horizontalOffset * 2, 15), "", GUI.skin.horizontalSlider);

            GUI.backgroundColor = new Color(0f, 0f, 0.5f);
            GUI.skin.button.normal.textColor = Color.white;

            if (GUI.Button(new Rect(pos.x + horizontalOffset, pos.y + size.y - 50, size.x - horizontalOffset * 2, 20), "Simulate movement"))
            {
                simulating.Invoke();
            }

            if (GUI.Button(new Rect(pos.x + horizontalOffset, pos.y + size.y - 25, size.x - horizontalOffset * 2, 20), "Spawn platforms!"))
            {
                spawning.Invoke();
            }

            Handles.EndGUI();

        }
    }

}
