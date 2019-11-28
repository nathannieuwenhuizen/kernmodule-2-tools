using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CharacterMovementCreator
{
    /// <summary>
    /// Places objects with a sprite and box2d collider for the character movement to test their movement.
    /// </summary>
    public class PlatformPlacer
    {
        //parent object on where the objects arep laces
        public GameObject parent;
        static string parentName = "Platforms_parent";
        public float heightOffset = 0;
        //places the objects based on the positions given in the paremter
        public void PlacePlatformObjects(List<Vector3> positions, BoxCollider2D coll, List<Vector2> sizes)
        {
            //create an empty parent object if there is none.
            parent = GameObject.Find(parentName);
            if (parent == null)
            {
                parent = new GameObject(parentName);
            }
           
            heightOffset = coll.size.y / 2;
            //instantiates the platforms
            for (int i = 0; i < positions.Count; i++)
            {
                PlaceGround(positions[i], sizes[i]);
            }
        }

        public void PlaceGround(Vector3 pos, Vector2 size, bool rounded = true)
        {
            //declaration
            GameObject obj = new GameObject("platform");

            //adding the components
            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/CharacterMovement/Sprites/test_sprite.jpg", typeof(Sprite));
            obj.AddComponent<BoxCollider2D>();

            //sets the size of the platform
            SetSize(size, sr);

            //position and hierarchy
            if (parent == null)
            {
                //create an empty parent object if there is none.
                parent = GameObject.Find(parentName);
                if (parent == null)
                {
                    parent = new GameObject(parentName);
                }
            }
            obj.transform.SetParent(parent.transform);

            pos.y -= heightOffset / 2 + obj.GetComponent<BoxCollider2D>().size.y / 2;
            if (rounded)
            {
                obj.transform.position = RoundToDecimals(pos, 2);
            } else
            {
                obj.transform.position = pos;
            }
        }
        private void SetSize(Vector2 desiredSize, SpriteRenderer sr)
        {
            Vector2 normalSize = new Vector2();
            normalSize.x = sr.sprite.textureRect.width / sr.sprite.pixelsPerUnit;
            normalSize.y = sr.sprite.textureRect.height / sr.sprite.pixelsPerUnit;
            Debug.Log(normalSize);
            sr.transform.localScale = new Vector3(desiredSize.x / normalSize.x, desiredSize.y / normalSize.x);
        }
        //round to decimals
        static Vector3 RoundToDecimals(Vector3 val, int amount)
        {
            float pow = Mathf.Pow(10, amount);
            return new Vector3(Mathf.Round(val.x * pow) / pow, Mathf.Round(val.y * pow) / pow, Mathf.Round(val.z * pow) / pow);
        }



    }

}