using UnityEngine;

public class EnemyMovement : MonoBehaviour {
  [Header("Chase Player")] [SerializeField]
  bool shouldChasePlayer = false;

  GameObject player;

  [Header("Movement")] [SerializeField] float moveSpeed = 1f;
  Rigidbody2D myRigidbody;

  void Start() {
    myRigidbody = GetComponent<Rigidbody2D>();

    GetPlayer();
  }

  void Update() {
    myRigidbody.velocity = new Vector2(moveSpeed, 0f);

    if (shouldChasePlayer) {
      GetPlayer();
      ChasePlayer();
    }
  }

  void OnTriggerExit2D(Collider2D other) {
    moveSpeed = -moveSpeed;
    FlipEnemyFacing();
  }

  void FlipEnemyFacing() {
    transform.localScale = new Vector2(Mathf.Sign(moveSpeed), 1f);
  }

  void ChasePlayer() {
    // if the distance between the player and the enemy is less than 5 units
    var distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
    if (distanceToPlayer > 5f) return;
    moveSpeed = player.transform.position.x > transform.position.x ? 1f : -1f;
    FlipEnemyFacing();
  }

  void GetPlayer() {
    if (!player) {
      player = GameObject.FindWithTag("Player");
    }
  }
}