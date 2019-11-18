using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public Rigidbody projectile;
    public Vector3 offset = Vector3.forward;
    public float velocity = 10;

    public void Fire()
    {
        var body = Instantiate(
            projectile,
            transform.TransformPoint(offset),
            transform.rotation);
        body.velocity = Vector3.forward * velocity;
    }
}