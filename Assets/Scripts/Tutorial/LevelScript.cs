using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class LevelScript : MonoBehaviour
{
    public GameObject obj;
    public Vector3 spawnPoint;
    public void BuildObject()
    {
        GameObject temp = Instantiate(obj, spawnPoint, Quaternion.identity);
        temp.name = "shit!";
    }

    [HideInInspector] public Rigidbody rigidbody;
    void Reset()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    [ContextMenu("Fire")]
    public void Fire()
    {
    }
}
