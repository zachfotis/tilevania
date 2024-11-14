using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour {
  [SerializeField] int coinValue = 100;
  [SerializeField] AudioClip coinPickupSFX;
  bool hasBeenPickedUp = false;
  
  private void OnTriggerEnter2D(Collider2D other) {
    if (!other.CompareTag("Player") || hasBeenPickedUp) return;
    
    hasBeenPickedUp = true;
    FindObjectOfType<GameSession>().AddToScore(coinValue);
    if (Camera.main != null) AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
    Destroy(gameObject);
  }
}