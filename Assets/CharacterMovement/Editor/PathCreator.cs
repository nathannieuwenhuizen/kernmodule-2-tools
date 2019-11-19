using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class PathCreator
{ 
    static float fps = 60;
    static public List<Vector3> jumpArc(MovementClass movementClass)
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
        using (new Handles.DrawingScope(Color.yellow))
        {
            Handles.DrawAAPolyLine(positions.ToArray());
            Gizmos.DrawWireSphere(positions[positions.Count - 1], 0.125f);
            float xDistance = positions[positions.Count - 1].x - movementClass.transform.position.x;
            Handles.Label(positions[positions.Count - 1], "Single Jump ( " + RoundToDecimals(xDistance, 2) + " units )");
        }
        return positions;
    }

    static float RoundToDecimals(float val, int amount)
    {
        float pow = Mathf.Pow(10, amount);
        return Mathf.Round(val * pow) / pow;
    }

    static public void normalJump(MovementClass movementClass)
    {
        float heigth = jumpHeight(movementClass);
        var offsetPosition = movementClass.transform.position + new Vector3(0, heigth, 0); // (movementClass.transform.po);
        using (new Handles.DrawingScope(Color.yellow))
        {
            Handles.DrawDottedLine(movementClass.transform.position, offsetPosition, 3);
            Gizmos.DrawWireSphere(offsetPosition, 0.125f);
            Handles.Label(offsetPosition, "Jump height ( " + RoundToDecimals(heigth, 2) + " units )");
        }
    }
    static float jumpHeight(MovementClass movementClass)
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

}
