using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
  [SerializeField] float bulletSpeed = 10f;
  Rigidbody2D myRigidody;
  PlayerMovement player;
  float xSpeed;

  void Start() {
    myRigidody = GetComponent<Rigidbody2D>();
    player = FindObjectOfType<PlayerMovement>();
    xSpeed = player.transform.localScale.x * bulletSpeed;
    transform.localScale = new Vector2(Mathf.Sign(xSpeed), 1f); 
  }

  void Update() {
    myRigidody.velocity = new Vector2(xSpeed, 0f);
  }
  
  void OnTriggerEnter2D(Collider2D other) {
    Destroy(gameObject);
    if(other.CompareTag($"Enemy")) {
      Destroy(other.gameObject);
    }
  }
}