using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using System;
using UnityEditor;

public class CharacterMaker : MonoBehaviour
{
    public static void GenerateScript(ClassData _data, bool newObject = false)
    {
        Debug.Log("generate...");

        StringBuilder strBuilder = new StringBuilder();
        Include(strBuilder);
        Header(strBuilder, _data);
        Body(strBuilder, _data);
        Footer(strBuilder);

        using (StreamWriter sWriter = new StreamWriter( _data.path + "/" + _data.className + ".cs"))
        {
            sWriter.WriteLine(strBuilder.ToString());
            sWriter.Flush();
            sWriter.Close();

            Debug.Log("Generated!");
        }

        if (newObject)
        {
            MakeGameObject(_data);
        } else
        {

        }
    }
    public static void Include(StringBuilder sb)
    {
        sb.AppendLine("using System.Collections;");
        sb.AppendLine("using UnityEngine;");
    }
    public static void Header(StringBuilder sb, ClassData _data)
    {
        sb.AppendLine("[RequireComponent(typeof(Rigidbody2D))]");
        sb.AppendLine("[RequireComponent(typeof(BoxCollider2D))]");
        sb.AppendLine("public class MovementClass : MonoBehaviour");
        sb.AppendLine("{");  
    }
    public static void Body(StringBuilder sb, ClassData _data)
    {
        Debug.Log("friction " +FloattoString(_data.friction));   
        sb.AppendLine($"public float walkSpeed = {FloattoString(_data.walkspeed)}; ");
        sb.AppendLine($"private float friction = {FloattoString(_data.friction)}; ");
        sb.AppendLine($"public float jumpSpeed = {FloattoString(_data.jumpSpeed)}; ");
        sb.AppendLine($"public float gravityScale = {FloattoString(_data.gravityScale)}; ");
        sb.AppendLine($"public float maxFallSpeed = {FloattoString(_data.maxFallSpeed)}; ");
        sb.AppendLine($"private float justInTimeDurationOnGround = {FloattoString(_data.justInTimeDurationOnGround)};");
        sb.AppendLine("\tprivate Vector2 deltaMovement = new Vector2();");
        sb.AppendLine("\tprivate Rigidbody2D rb; ");
        sb.AppendLine("\tprivate bool inAir = true;");
        sb.AppendLine("\t ");
        sb.AppendLine("\tvoid Start() { ");
        sb.AppendLine("\trb = GetComponent<Rigidbody2D>(); rb.gravityScale = 0; } ");
        sb.AppendLine("\t");
        sb.AppendLine("\tprivate void Move(float _input) { deltaMovement.x = _input * walkSpeed; }");
        sb.AppendLine("\t ");
        sb.AppendLine("\tprivate void FixedUpdate() { if (inAir) { deltaMovement.y = Mathf.Max(-maxFallSpeed, deltaMovement.y - gravityScale); } ");
        sb.AppendLine("\t");
        sb.AppendLine("\trb.velocity = deltaMovement; } ");
        sb.AppendLine("\t");
        sb.AppendLine("\tprivate void OnCollisionEnter2D(Collision2D collision) { StopAllCoroutines(); inAir = false; deltaMovement.y = 0; } ");
        sb.AppendLine("\t");
        sb.AppendLine("\tprivate void OnCollisionExit2D(Collision2D collision) { StartCoroutine(DelayInAir()); }");
        sb.AppendLine("\t ");
        sb.AppendLine("\tprivate IEnumerator DelayInAir() { ");
        sb.AppendLine("\tyield return new WaitForSeconds(justInTimeDurationOnGround); inAir = true; }");
        sb.AppendLine("\t ");
        sb.AppendLine("\tprivate void Jump() { ");
        sb.AppendLine("\tif (!inAir) { deltaMovement.y = jumpSpeed; inAir = true; } }");
        sb.AppendLine("\t private void Update() { Move(Input.GetAxis(\"Horizontal\")); if (Input.GetButtonDown(\"Fire1\")) { Jump(); } } ");
        sb.AppendLine("\t");

    }
    public static void Footer(StringBuilder sb)
    {
        sb.AppendLine("}");
    }
    
    public static string FloattoString(float val)
    {
        return val.ToString().Replace(",", ".") + "f";
    }


    public static void MakeGameObject(ClassData _data)
    {
        GameObject obj = new GameObject("character");
        obj.transform.position = new Vector3(0, 0, 0);
        obj.AddComponent<SpriteRenderer>();
        obj.GetComponent<SpriteRenderer>().sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/CharacterMovement/test_sprite.jpg", typeof(Sprite));
        obj.AddComponent(Type.GetType(_data.className));
        //obj.AddComponent<Movem>

        // TODO: chech physics setting in unity for gizmo.
        //Physics.gravity
    }
}