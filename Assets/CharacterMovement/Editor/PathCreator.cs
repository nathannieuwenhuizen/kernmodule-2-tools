using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The path creator creates the paths and arcs of the character movement
/// </summary>
/// 
namespace CharacterMovementCreator
{
    public static class PathCreator
    {
        static float fps = 54;
        static public float xJumpDistance;
        static public float xDoubleJumpDistance;

        //Calculates and draws the jump arc
        static public List<Vector3> JumpArc(UniqueMovement movementClass)
        {
            List<Vector3> positions = GetJumpArc(movementClass);
            using (new Handles.DrawingScope(Color.yellow))
            {
                Handles.DrawAAPolyLine(positions.ToArray());
                xJumpDistance = positions[positions.Count - 1].x - movementClass.transform.position.x;

                LabelUnits("Single jump", positions[positions.Count - 1], xJumpDistance);
            }
            return positions;
        }

        //Calculates and draws the jump arc
        static public List<Vector3> DoubleJumpArc(UniqueMovement movementClass)
        {
            List<Vector3> positions = GetDoubleJumpArc(movementClass);
            using (new Handles.DrawingScope(Color.yellow))
            {
                Handles.DrawAAPolyLine(positions.ToArray());
                xDoubleJumpDistance = positions[positions.Count - 1].x - movementClass.transform.position.x;

                LabelUnits("Double jump", positions[positions.Count - 1], xDoubleJumpDistance);
            }
            return positions;
        }

        //calculates and returns the jump arc of a given movement
        static List<Vector3> GetJumpArc(UniqueMovement movementClass)
        {
            Vector3 pos = movementClass.transform.position;
            float deltaY = movementClass.jumpSpeed;
            float deltaX = movementClass.walkSpeed;
            List<Vector3> positions = new List<Vector3>();

            if (movementClass.gravityScale > 0)
            {
                while (pos.y >= movementClass.transform.position.y && movementClass.gravityScale != 0)
                {
                    positions.Add(pos);
                    pos.y += deltaY / fps;
                    deltaY -= movementClass.gravityScale;

                    if (movementClass.maxFallSpeed != 0 && deltaY < -movementClass.maxFallSpeed && movementClass.maxFallSpeed > 0)
                    {
                        deltaY = -movementClass.maxFallSpeed;
                    }
                    pos.x += deltaX / fps;
                }
                pos.x -= (deltaX / fps) * ((pos.y - movementClass.transform.position.y) / (deltaY / fps));
                pos.y = movementClass.transform.position.y;
                positions.Add(pos);
            }
            return positions;
        }


        //calculates and returns the double jump arc of a given movement
        static List<Vector3> GetDoubleJumpArc(UniqueMovement movementClass)
        {
            Vector3 pos = movementClass.transform.position;
            float deltaY = movementClass.jumpSpeed;
            float deltaX = movementClass.walkSpeed;
            List<Vector3> positions = new List<Vector3>();
            int doublejumpIndex = 0;

            if (movementClass.gravityScale > 0)
            {
                while (pos.y >= movementClass.transform.position.y && movementClass.gravityScale != 0 )
                {
                    positions.Add(pos); 
                    pos.y += deltaY / fps;
                    deltaY -= movementClass.gravityScale;
                    if (movementClass.maxFallSpeed != 0 && deltaY < -movementClass.maxFallSpeed && movementClass.maxFallSpeed > 0)
                    {
                        deltaY = -movementClass.maxFallSpeed;
                    }

                    if (deltaY <= -(Mathf.Min(movementClass.jumpSpeed / 2, movementClass.maxFallSpeed)) && doublejumpIndex < Mathf.Min(10, movementClass.amountOfDoubleJumps))
                    {
                        //doublejump
                        deltaY = movementClass.doubleJumpSpeed;
                        doublejumpIndex++;
                    }
                    pos.x += deltaX / fps;
                }
                pos.x -= (deltaX / fps) * ((pos.y - movementClass.transform.position.y) / (deltaY / fps));
                pos.y = movementClass.transform.position.y;
                positions.Add(pos);
            }
            return positions;
        }



        //round to decimals
        static float RoundToDecimals(float val, int amount)
        {
            float pow = Mathf.Pow(10, amount);
            return Mathf.Round(val * pow) / pow;
        }

        //draws the idle jump
        static public void DrawIdleJump(UniqueMovement movementClass)
        {
            float heigth = GetJumpUnits(movementClass);
            var jumpPos = movementClass.transform.position + new Vector3(0, heigth, 0); // (movementClass.transform.po);
            using (new Handles.DrawingScope(Color.yellow))
            {
                Handles.DrawDottedLine(movementClass.transform.position, jumpPos, 3);
                LabelUnits("Jump height", jumpPos, heigth);
            }
        }

        //draws the double idle jump
        static public void DrawDoubleIdleJump(UniqueMovement movementClass)
        {
            float heigth = GetDoubleJumpUnits(movementClass);
            var jumpPos = movementClass.transform.position + new Vector3(0, heigth, 0);
            using (new Handles.DrawingScope(Color.yellow))
            {
                Handles.DrawDottedLine(movementClass.transform.position, jumpPos, 3);
                LabelUnits("Double jump height", jumpPos, heigth);
            }

        }

        static public void LabelUnits(string text, Vector3 pos, float units = 0)
        {
            string unitText = "";
            if (units != 0)
            {
                 unitText += " ( " + RoundToDecimals(units, 2) + " units )";
            }
            Handles.Label(pos, text + unitText);
            Gizmos.DrawWireSphere(pos, 0.125f);

        }

        //get the max vertical jump in units
        static public float GetJumpUnits(UniqueMovement movementClass)
        {
            float result = 0;
            float jumpSpeed = movementClass.jumpSpeed;
            if (movementClass.gravityScale > 0)
            {
                while (jumpSpeed > 0)
                {
                    result += jumpSpeed / fps;
                    jumpSpeed -= movementClass.gravityScale;
                }
            }
            return result;
        }
        //get the max vertical jump in units
        static public float GetDoubleJumpUnits(UniqueMovement movementClass)
        {
            int doublejumpIndex = 0;
            float result = 0;
            float deltaY = movementClass.jumpSpeed;

            if (movementClass.gravityScale > 0)
            {
                while (deltaY > 0 && doublejumpIndex <= Mathf.Min(10, movementClass.amountOfDoubleJumps))
                {
                    result += deltaY / fps;
                    deltaY -= movementClass.gravityScale;
                    if (deltaY <= 0)
                    {
                        //doublejump
                        deltaY = movementClass.doubleJumpSpeed;
                        doublejumpIndex++;
                    }
                }
            }
            return result;
        }

        //changes the speed of the character based of the val in units
        static public float SetJumpUnits(float val, UniqueMovement movementClass)
        {
            float jumpSpeed = 0;
            float pos = +val;
            if (movementClass.gravityScale > 0)
            {
                while (pos >= 0)
                {
                    jumpSpeed += movementClass.gravityScale / fps;
                    pos -= jumpSpeed;
                }
                jumpSpeed -= (movementClass.gravityScale / fps) * (-pos / jumpSpeed);
            }
            return jumpSpeed * fps;
        }

        //changes the speed of the character based of the val in units
        static public float SetDoubleJumpUnits(float val, UniqueMovement movementClass)
        {
            float jumpSpeed = 0;
            float pos = +val;
            if (movementClass.gravityScale > 0)
            {
                while (pos >= GetJumpUnits(movementClass))
                {
                    jumpSpeed += movementClass.gravityScale / fps;
                    pos -= jumpSpeed;
                }
                jumpSpeed -= (movementClass.gravityScale / fps) * (-pos / jumpSpeed);
            }
            return jumpSpeed * fps;
        }

    }
}
