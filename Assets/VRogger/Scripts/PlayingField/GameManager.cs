using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    private int _maxLifes = 5;
    private int _maxTime = 300;
    
    private int _currentLifes;
    private float _remainingTime;
    
    public bool isPaused { get; set; }
    public float score { get; set; }

    public float maxLifes { get { return _maxLifes; } }
    public float currentLifes { get { return _currentLifes; } }
    public float remainingTime{ get { return _remainingTime; } }

    
    public UnityEvent GameOver = new UnityEvent();
    public UnityEvent LifeLost = new UnityEvent();
    private GameManager() {
        // private constructor to ensure GameManager is singleton and not instantiated 
    }

    // Keep GM alive so stats stay consistent across different scenes (probably unneccessary) 
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        isPaused = true;
        StartGame();
        LifeLost.AddListener(ReduceLifes);

    }

    private void Update() {
        if (isPaused) return;

        if (remainingTime > 0) {
            _remainingTime -= Time.deltaTime;
        } else
        {
            isPaused = true;
            GameOver.Invoke();
        }
    }


    public void LoadLevel(int level) {
        if (level < SceneManager.sceneCountInBuildSettings) {
            SceneManager.LoadScene(level);
        }
    }

    public void StartGame() {
        _currentLifes = _maxLifes;
        score = 0;
        _remainingTime = _maxTime;
        isPaused = true;
        LoadLevel(1);
    }

    public void ReduceLifes() {
        _currentLifes--;

        if (_currentLifes == 0) {
            GameOver.Invoke();
            isPaused = true;
        }
    }

}
