using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The path creator creates the paths and arcs of the character movement
/// </summary>
/// 
public static class PathCreator
{ 
    static float fps = 54;
    static public float xDistance;

    //Calculates and draws the jump arc
    static public List<Vector3> JumpArc(MovementClass movementClass)
    {
        List<Vector3> positions = GetJumpArc(movementClass);
        using (new Handles.DrawingScope(Color.yellow))
        {
            Handles.DrawAAPolyLine(positions.ToArray());
            Gizmos.DrawWireSphere(positions[positions.Count - 1], 0.125f);
            xDistance = positions[positions.Count - 1].x - movementClass.transform.position.x;
            Handles.Label(positions[positions.Count - 1], "Single Jump ( " + RoundToDecimals(xDistance, 2) + " units )");
        }
        return positions;
    }

    //calculates and returns the jump arc of a given movement
    static List<Vector3> GetJumpArc(MovementClass movementClass)
    {
        Vector3 pos = movementClass.transform.position;
        float jumpSpeed = movementClass.jumpSpeed;
        float walkSpeed = movementClass.walkSpeed;
        List<Vector3> positions = new List<Vector3>();

        if (movementClass.gravityScale > 0)
        {
            while (pos.y >= movementClass.transform.position.y && movementClass.gravityScale != 0)
            {
                positions.Add(pos);
                pos.y += jumpSpeed / fps;
                jumpSpeed -= movementClass.gravityScale;

                if (movementClass.maxFallSpeed != 0 && jumpSpeed < -movementClass.maxFallSpeed && movementClass.maxFallSpeed > 0)
                {
                    jumpSpeed = -movementClass.maxFallSpeed;
                }
                pos.x += walkSpeed / fps;
            }
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
    static public void DrawIdleJump(MovementClass movementClass)
    {
        float heigth = GetJumpUnits(movementClass);
        var jumpPos = movementClass.transform.position + new Vector3(0, heigth, 0); // (movementClass.transform.po);
        using (new Handles.DrawingScope(Color.yellow))
        {
            Handles.DrawDottedLine(movementClass.transform.position, jumpPos, 3);
            Gizmos.DrawWireSphere(jumpPos, 0.125f);
            Handles.Label(jumpPos, "Jump height ( " + RoundToDecimals(heigth, 2) + " units )");
        }
    }

    //get the max vertical jump in units
    static public float GetJumpUnits(MovementClass movementClass)
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

    //changes the speed of the character based of the val in units
    static public float SetJumpUnits(float val, MovementClass movementClass)
    {
        float jumpSpeed = 0;
        float pos =  + val;
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

}
