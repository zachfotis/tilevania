using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
  [SerializeField] private float runSpeed = 5f;
  [SerializeField] private float jumpSpeed = 5f;
  [SerializeField] private Vector2 deathKick = new Vector2(20f, 20f);
  [SerializeField] private GameObject bullet;
  [SerializeField] private GameObject gun;
  
  Vector2 moveInput;
  Rigidbody2D rb;
  Animator myAnimator;
  CapsuleCollider2D myBodyCollider;
  BoxCollider2D myFeetCollider;
  float gravityScaleAtStart;
  [SerializeField] bool isAlive = true;

  void Start() {
    rb = GetComponent<Rigidbody2D>();
    myAnimator = GetComponent<Animator>();
    myBodyCollider = GetComponent<CapsuleCollider2D>();
    myFeetCollider = GetComponent<BoxCollider2D>();
    gravityScaleAtStart = rb.gravityScale;
  }

  void Update() {
    if (!isAlive) return;
    Run();
    FlipSprite();
    ClimbLadder();
    Die();
  }
  
  void OnFire(InputValue value) {
    if (!isAlive) return;
    Instantiate(bullet, gun.transform.position, gun.transform.rotation);
  }

  // This function is available due to the Input System on the Player object
  void OnMove(InputValue value) {
    if (!isAlive) return;
    moveInput = value.Get<Vector2>();
  }

  void OnJump(InputValue value) {
    if (!isAlive) return;
    if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;

    if (value.isPressed) {
      rb.velocity += new Vector2(0f, jumpSpeed);
    }
  }

  void Run() {
    Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
    rb.velocity = playerVelocity;

    bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
    myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
  }

  void FlipSprite() {
    bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
    if (playerHasHorizontalSpeed) {
      transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1);
    }
  }

  void ClimbLadder() {
    if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
      rb.gravityScale = gravityScaleAtStart;

      myAnimator.SetBool("isClimbing", false);
      return;
    }

    Vector2 climbVelocity = new Vector2(rb.velocity.x, moveInput.y * runSpeed);
    rb.velocity = climbVelocity;
    rb.gravityScale = 0f;

    bool playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
    myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
  }

  void Die() {
    if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"))) {
      isAlive = false;
      myAnimator.SetTrigger("Dying");
      rb.velocity = deathKick;
      FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }
  }
}