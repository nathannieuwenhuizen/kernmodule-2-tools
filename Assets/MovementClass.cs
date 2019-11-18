using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class MovementClass : MonoBehaviour
{
private float walkSpeed = 20f; 
private float friction = 0.1f; 
[SerializeField] private float jumpSpeed = 5f; 
private float gravityScale = 0.1f; 
private float maxFallSpeed = 3f; 
private float justInTimeDurationOnGround = 100f;
	private Vector2 deltaMovement = new Vector2();
	private Rigidbody2D rb; 
	private bool inAir = true;
	 
	void Start() { 
	rb = GetComponent<Rigidbody2D>(); rb.gravityScale = 0; } 
	
	private void Move(float _input) { deltaMovement.x = _input * walkSpeed; }
	 
	private void FixedUpdate() { if (inAir) { deltaMovement.y = Mathf.Max(-maxFallSpeed, deltaMovement.y - gravityScale); } 
	
	rb.velocity = deltaMovement; } 
	
	private void OnCollisionEnter2D(Collision2D collision) { StopAllCoroutines(); inAir = false; deltaMovement.y = 0; } 
	
	private void OnCollisionExit2D(Collision2D collision) { StartCoroutine(DelayInAir()); }
	 
	private IEnumerator DelayInAir() { 
	yield return new WaitForSeconds(justInTimeDurationOnGround); inAir = true; }
	 
	private void Jump() { 
	if (!inAir) { deltaMovement.y = jumpSpeed; inAir = true; } }
	 private void Update() { Move(Input.GetAxis("Horizontal")); if (Input.GetButtonDown("Fire1")) { Jump(); } } 
	
}

