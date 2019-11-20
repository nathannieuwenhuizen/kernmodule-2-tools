using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Places objects with a sprite and box2d collider for the character movement to test their movement.
/// </summary>
public class PlatformPlacer : MonoBehaviour
{
    //parent object on where the objects arep laces
    public GameObject parent;
    static string parentName = "Platforms_parent";
    public float heightOffset = 0;
    //places the objects based on the positions given in the paremter
    public void PlacePlatformObjects(List<Vector3> positions, BoxCollider2D coll)
    {
        //create an empty parent object if there is none.
        parent = GameObject.Find(parentName);
        if (parent == null)
        {
            parent = new GameObject(parentName);
        }

        heightOffset = coll.size.y / 2;
        //instantiates the platforms
        for (int i = 0; i< positions.Count; i++)
        {
            PlaceGround(positions[i]);
        }
    }

    private void PlaceGround(Vector3 pos)
    {
        //declaration
        GameObject obj = new GameObject("platform");

        //adding the components
        obj.AddComponent<SpriteRenderer>();
        obj.GetComponent<SpriteRenderer>().sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/CharacterMovement/test_sprite.jpg", typeof(Sprite));
        obj.AddComponent<BoxCollider2D>();

        //scaling to make it more like a ceratian platform or wall
        obj.transform.localScale = new Vector3(3f, 0.5f);

        //position and hierarchy
        obj.transform.SetParent(parent.transform);
        pos.y -= heightOffset / 2 + obj.GetComponent<BoxCollider2D>().size.y / 2;
        obj.transform.position = pos;

    }
    private void ClearPlatforms()
    {

    }

}
