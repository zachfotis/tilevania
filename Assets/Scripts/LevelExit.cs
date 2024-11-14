using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour {
  [SerializeField] float levelLoadDelay = 1f;
  [SerializeField] float levelExitSlowMoFactor = 0.2f;

  void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Player")) {
      StartCoroutine(LoadNextLevel());
    }
  }

  IEnumerator LoadNextLevel() {
    Time.timeScale = levelExitSlowMoFactor;
    yield return new WaitForSecondsRealtime(levelLoadDelay);
    Time.timeScale = 1f;

    var currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    var nextLevelIndex = currentLevelIndex + 1;

    if (nextLevelIndex == SceneManager.sceneCountInBuildSettings) {
      FindObjectOfType<GameSession>().ResetGameSession();
      nextLevelIndex = 0;
    }

    FindObjectOfType<ScenePersist>().ResetScenePersist();
    SceneManager.LoadScene(nextLevelIndex);
  }
}