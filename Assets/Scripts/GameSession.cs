using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour {
  [SerializeField] int playerLives = 3;
  [SerializeField] int score = 0;

  [SerializeField] TextMeshProUGUI livesText;
  [SerializeField] TextMeshProUGUI scoreText;

  void Awake() {
    int numGameSessions = FindObjectsOfType<GameSession>().Length;

    if (numGameSessions > 1) {
      Destroy(gameObject);
    } else {
      DontDestroyOnLoad(gameObject);
    }
  }

  void Start() {
    livesText.text = playerLives.ToString();
    scoreText.text = score.ToString();
  }

  private void Update() {
    // if the player presses the "L" key, take a life
    if (Input.GetKeyDown(KeyCode.L)) {
      AddToLives(1);
    }

    // if the player presses the "ESC" key, quit the game
    if (Input.GetKeyDown(KeyCode.Escape)) {
      Application.Quit();
    }
  }

  public void AddToScore(int points) {
    score += points;
    scoreText.text = score.ToString();
  }

  public void ProcessPlayerDeath() {
    if (playerLives > 1) {
      TakeLife();
    } else {
      ResetGameSession();
    }
  }

  private void AddToLives(int lives) {
    playerLives += lives;
    livesText.text = playerLives.ToString();
  }

  private void TakeLife() {
    playerLives--;
    livesText.text = playerLives.ToString();
    var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    SceneManager.LoadScene(currentSceneIndex);
  }

  public void ResetGameSession() {
    FindObjectOfType<ScenePersist>().ResetScenePersist();
    SceneManager.LoadScene(0);
    Destroy(gameObject);
  }
}