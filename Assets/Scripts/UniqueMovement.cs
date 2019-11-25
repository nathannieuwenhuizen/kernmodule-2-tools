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
    public bool jumpEnabled = true;
    public float jumpSpeed = 5f;
    public float gravityScale = 0.1f;
    public float maxFallSpeed = 3f;
    public float justInTimeDurationOnGround = 0.1f;

    //doublejumps
    public bool doubleJumpEnabled = true;
    public int amountOfDoubleJumps = 30; 
    public float doubleJumpSpeed = 10f;
    private int doubleJumpIndex = 0;

    //sprite
    public bool faceToDirection = true;
    public Vector3 originScale;

    //crouch
    public bool crouchEnabled = false;
    public bool crouching = false;
    public float crouchSpeed = 4f;
    public float crouchScale = 0.5f;

    //baisc values
    private Vector2 deltaMovement = new Vector2();
    private Rigidbody2D rb;
    private bool inAir = true;

    void Start()
    {
        originScale = transform.localScale;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
    private void Move(float _input)
    {
        if (crouching)
        {
            deltaMovement.x = _input * crouchSpeed;
        }
        else
        {
            deltaMovement.x = _input * walkSpeed;
        }

        if (deltaMovement.x != 0)
        {
            transform.localScale = new Vector2(deltaMovement.x < 0 ? -originScale.x : originScale.x, transform.localScale.y);
        }
    }
    private void FixedUpdate()
    {
        if (inAir)
        {
            deltaMovement.y = Mathf.Max(-maxFallSpeed, deltaMovement.y - gravityScale);
        }
        rb.velocity = deltaMovement;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopAllCoroutines();
        inAir = false;
        deltaMovement.y = 0;
        doubleJumpIndex = 0;
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
        //normal jump
        if (!inAir && jumpEnabled)
        {
            deltaMovement.y = jumpSpeed;
            inAir = true;
        }
        //double jump 
        else if (doubleJumpEnabled && doubleJumpIndex < amountOfDoubleJumps)
        {
            doubleJumpIndex++;
            deltaMovement.y = doubleJumpSpeed;
        }
    }

    private void Update()
    {

        Move(Input.GetAxis("Horizontal"));
        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            crouching = true;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            crouching = false;
        }
        transform.localScale = new Vector2( transform.localScale.x, originScale.y * (crouching ? crouchScale : 1));


    }

}
