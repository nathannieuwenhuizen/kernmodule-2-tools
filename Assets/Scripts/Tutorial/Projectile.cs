using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] new public Rigidbody rigidbody;
    public float damageRadius = 1;

    void Reset()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
}