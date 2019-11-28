using System.Collections;
using UnityEngine;


public enum dashModes
{
    horizontalLine,
    cross,
    every_angle
}
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

    //dash
    public bool enableDash = false;
    public float dashSpeed = 50f;
    public float dashDuration = 0.2f;
    private bool dashing = false;
    public dashModes dashMode = dashModes.horizontalLine;

    void Start()
    {
        originScale = transform.localScale;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
    private void Move(float _input)
    {
        if (dashing) { return; }

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
        if (inAir && !dashing)
        {
            deltaMovement.y = Mathf.Max(-maxFallSpeed, deltaMovement.y - gravityScale);
        }
        rb.velocity = deltaMovement;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
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

    private void Dash(Vector2 input)
    {
        if (dashing || !enableDash) { return; }
        dashing = true;

        deltaMovement = new Vector2(0, 0);
        if (input.x == 0)
        {
            if (transform.localScale.x < 0)
            {
                deltaMovement.x = -dashSpeed;
            } else
            {
                deltaMovement.x = dashSpeed;
            }
        } 
        switch (dashMode) {
            case (dashModes.horizontalLine):
                if (input.x < 0)
                {
                    deltaMovement.x = -dashSpeed;
                } else
                {
                    deltaMovement.x = dashSpeed;
                }
                break;
            case (dashModes.cross):
                bool vertical = Mathf.Abs(input.x) < Mathf.Abs(input.y);
                if (vertical)
                {
                    if (input.y < 0)
                    {
                        deltaMovement.y = -dashSpeed;
                    }
                    else
                    {
                        deltaMovement.y = dashSpeed;
                    }
                }
                else
                {
                    if (input.x < 0)
                    {
                        deltaMovement.x = -dashSpeed;
                    }
                    else
                    {
                        deltaMovement.x = dashSpeed;
                    }
                }
                break;
            case (dashModes.every_angle):
                Vector2 normalized = input.normalized;
                deltaMovement.x = normalized.x * dashSpeed;
                deltaMovement.y = normalized.y * dashSpeed;
                break;
        }
        StartCoroutine(Dashing());
    }
    IEnumerator Dashing()
    {
        yield return new WaitForSeconds(dashDuration);
        StopDash();
    }
    private void StopDash()
    {
        deltaMovement.x = 0;
        deltaMovement.y = 0;
        dashing = false;
    }

    private void Update()
    {

        Move(Input.GetAxis("Horizontal"));
        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.S) && !inAir)
        {
            crouching = true;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            crouching = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dash(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        }

        transform.localScale = new Vector2( transform.localScale.x, originScale.y * (crouching ? crouchScale : 1));


    }

}
