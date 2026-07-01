using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnsurePreload : MonoBehaviour {
    private void Awake() {
        if (GameManager.instance == null) {
            Debug.Log("GameManager not found, loading Preload scene");
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
        }
    }
}
