using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class UniqueMovement : MonoBehaviour
{

    //inspector values
    public float walkSpeed = 20f;
    private float friction = 0.1f;

    //jump
    public float jumpSpeed = 5f;
    public float gravityScale = 0.1f;
    public float maxFallSpeed = 3f;
    public float justInTimeDurationOnGround = 0.1f;

    //baisc values
    private Vector2 deltaMovement = new Vector2();
    private Rigidbody2D rb;
    private bool inAir = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
    private void Move(float _input)
    {
        deltaMovement.x = _input * walkSpeed;
    }
    private void FixedUpdate()
    {
        if (inAir)
        {
            deltaMovement.y = Mathf.Max(-maxFallSpeed, deltaMovement.y - gravityScale);
        }
        rb.velocity = deltaMovement;
        //transform.Translate(deltaMovement);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopAllCoroutines();
        inAir = false;
        deltaMovement.y = 0;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        StartCoroutine(DelayInAir());
    }
    private IEnumerator DelayInAir()
    {
        yield return new WaitForSeconds(justInTimeDurationOnGround);
        inAir = true;
    }

    private void Jump()
    {
        if (!inAir)
        {
            deltaMovement.y = jumpSpeed;
            inAir = true;
        }
    }

    private void Update()
    {
        Move(Input.GetAxis("Horizontal"));
        if (Input.GetButtonDown("Fire1"))
        {
            Jump();
        }
    }

}
